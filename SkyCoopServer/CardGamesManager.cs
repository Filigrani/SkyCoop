using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class CardGamesManager
    {
        public static Dictionary<string, TexasHoldEmGame> m_Games = new Dictionary<string, TexasHoldEmGame>();

        public static bool m_AllowJoinMultipleTimes = true;

        public static void StartNewGame(string GUID, string Scene, Server ServerInstance)
        {
            m_Games.Add(GUID, new TexasHoldEmGame(ServerInstance, GUID, Scene, 4, 1000));
        }

        public static void TryDoAction(string GUID, int GamePlayerID, string Action, int Amount = 0)
        {
            if(m_Games.ContainsKey(GUID))
            {
                m_Games[GUID].PlayerAction(GamePlayerID, Action, Amount);
            }
        }

        public static void TryJoinGame(NetPeer Client, string GUID, int PlayerID, int PokerID, Server ServerInstance)
        {
            SkyCoopServer.Logger.Log($"CardGamesManager TryJoinGame {GUID} ClinetID {PlayerID} GamePlayerID {PokerID}");
            if (m_Games.ContainsKey(GUID))
            {
                //m_Games[GUID].PlayerAction();

                if (!m_AllowJoinMultipleTimes)
                {
                    foreach (int playerID in m_Games[GUID].m_PokerIDToPlayerID.Values.ToArray())
                    {
                        if (playerID == PlayerID) // This player Already in game.
                        {
                            return;
                        }
                    }
                }
                if(m_Games[GUID].m_PokerIDToPlayerID.ContainsKey(PokerID)) // This player slot is busy.
                {
                    return;
                }
                SkyCoopServer.Logger.Log($"CardGamesManager TryJoinGame {GUID} ClinetID {PlayerID} GamePlayerID {PokerID}");
                m_Games[GUID].SetNewPlayer(PokerID, PlayerID);
                string SceneName = ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene;
                foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
                {
                    if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                    {
                        ServerSend.SendPlayerJoinCardGame(Peer, GUID, PlayerID, PokerID);
                    }
                }
            }
        }
        
        public class PokerHandEvaluator
        {
            public static (HandRank rank, List<CardType> kickers) EvaluateHand(List<PlayingCard> cards)
            {
                if (cards.Count < 5)
                    throw new ArgumentException("Need at least 5 cards to evaluate a hand");

                // Group cards by type and suit for easier evaluation
                var groupsByType = cards.GroupBy(c => c.m_Type)
                                      .OrderByDescending(g => g.Count())
                                      .ThenByDescending(g => g.Key)
                                      .ToList();

                var groupsBySuit = cards.GroupBy(c => c.m_Suit)
                                      .OrderByDescending(g => g.Count())
                                      .ToList();

                bool isFlush = groupsBySuit[0].Count() >= 5;
                var flushSuit = isFlush ? groupsBySuit[0].Key : (CardSuit?)null;

                // Check for straight
                var distinctTypes = cards.Select(c => c.m_Type).Distinct().OrderByDescending(t => t).ToList();
                bool isStraight = false;
                List<CardType> straightCards = new List<CardType>();

                // Check for regular straight (A-5 is handled separately)
                for (int i = 0; i <= distinctTypes.Count - 5; i++)
                {
                    if ((int)distinctTypes[i] - (int)distinctTypes[i + 4] == 4)
                    {
                        isStraight = true;
                        straightCards = distinctTypes.Skip(i).Take(5).ToList();
                        break;
                    }
                }

                // Check for wheel straight (A-2-3-4-5)
                if (!isStraight && distinctTypes.Contains(CardType.Ace) && distinctTypes.Contains(CardType.Two) &&
                    distinctTypes.Contains(CardType.Three) && distinctTypes.Contains(CardType.Four) &&
                    distinctTypes.Contains(CardType.Five))
                {
                    isStraight = true;
                    straightCards = new List<CardType> { CardType.Five, CardType.Four, CardType.Three, CardType.Two, CardType.Ace };
                }

                // Check for straight flush or royal flush
                if (isStraight && isFlush)
                {
                    var flushCards = cards.Where(c => c.m_Suit == flushSuit)
                                        .Select(c => c.m_Type)
                                        .OrderByDescending(t => t)
                                        .ToList();

                    bool isStraightFlush = false;
                    List<CardType> straightFlushCards = new List<CardType>();

                    // Check for regular straight flush
                    for (int i = 0; i <= flushCards.Count - 5; i++)
                    {
                        if ((int)flushCards[i] - (int)flushCards[i + 4] == 4)
                        {
                            isStraightFlush = true;
                            straightFlushCards = flushCards.Skip(i).Take(5).ToList();
                            break;
                        }
                    }

                    // Check for wheel straight flush (A-2-3-4-5)
                    if (!isStraightFlush && flushCards.Contains(CardType.Ace) && flushCards.Contains(CardType.Two) &&
                        flushCards.Contains(CardType.Three) && flushCards.Contains(CardType.Four) &&
                        flushCards.Contains(CardType.Five))
                    {
                        isStraightFlush = true;
                        straightFlushCards = new List<CardType> { CardType.Five, CardType.Four, CardType.Three, CardType.Two, CardType.Ace };
                    }

                    if (isStraightFlush)
                    {
                        // Check for royal flush
                        if (straightFlushCards.Contains(CardType.Ace) && straightFlushCards.Contains(CardType.King))
                        {
                            return (HandRank.RoyalFlush, straightFlushCards.Take(5).ToList());
                        }
                        return (HandRank.StraightFlush, straightFlushCards.Take(5).ToList());
                    }
                }

                // Four of a kind
                if (groupsByType[0].Count() == 4)
                {
                    var kickers = cards.Where(c => c.m_Type != groupsByType[0].Key)
                                     .OrderByDescending(c => c.m_Type)
                                     .Select(c => c.m_Type)
                                     .Take(1)
                                     .ToList();
                    return (HandRank.FourOfAKind, new List<CardType> { groupsByType[0].Key }.Concat(kickers).ToList());
                }

                // Full house
                if (groupsByType[0].Count() >= 3 && groupsByType.Count > 1 && groupsByType[1].Count() >= 2)
                {
                    var trips = groupsByType.Where(g => g.Count() >= 3).OrderByDescending(g => g.Key).First().Key;
                    var pair = groupsByType.Where(g => g.Count() >= 2 && g.Key != trips).OrderByDescending(g => g.Key).First().Key;
                    return (HandRank.FullHouse, new List<CardType> { trips, pair });
                }

                // Flush
                if (isFlush)
                {
                    var flushCards = cards.Where(c => c.m_Suit == flushSuit)
                                        .OrderByDescending(c => c.m_Type)
                                        .Select(c => c.m_Type)
                                        .Take(5)
                                        .ToList();
                    return (HandRank.Flush, flushCards);
                }

                // Straight
                if (isStraight)
                {
                    return (HandRank.Straight, straightCards.Take(5).ToList());
                }

                // Three of a kind
                if (groupsByType[0].Count() == 3)
                {
                    var kickers = cards.Where(c => c.m_Type != groupsByType[0].Key)
                                     .OrderByDescending(c => c.m_Type)
                                     .Select(c => c.m_Type)
                                     .Take(2)
                                     .ToList();
                    return (HandRank.ThreeOfAKind, new List<CardType> { groupsByType[0].Key }.Concat(kickers).ToList());
                }

                // Two pair
                if (groupsByType[0].Count() == 2 && groupsByType.Count > 1 && groupsByType[1].Count() == 2)
                {
                    var pairs = groupsByType.Where(g => g.Count() == 2)
                                          .OrderByDescending(g => g.Key)
                                          .Take(2)
                                          .Select(g => g.Key)
                                          .ToList();
                    var kicker = cards.Where(c => c.m_Type != pairs[0] && c.m_Type != pairs[1])
                                    .OrderByDescending(c => c.m_Type)
                                    .Select(c => c.m_Type)
                                    .First();
                    return (HandRank.TwoPair, new List<CardType> { pairs[0], pairs[1], kicker });
                }

                // Pair
                if (groupsByType[0].Count() == 2)
                {
                    var kickers = cards.Where(c => c.m_Type != groupsByType[0].Key)
                                     .OrderByDescending(c => c.m_Type)
                                     .Select(c => c.m_Type)
                                     .Take(3)
                                     .ToList();
                    return (HandRank.Pair, new List<CardType> { groupsByType[0].Key }.Concat(kickers).ToList());
                }

                // High card
                var highCards = cards.OrderByDescending(c => c.m_Type)
                                   .Select(c => c.m_Type)
                                   .Take(5)
                                   .ToList();
                return (HandRank.HighCard, highCards);
            }

            public static int CompareHands(List<PlayingCard> hand1, List<PlayingCard> hand2)
            {
                var evaluation1 = EvaluateHand(hand1);
                var evaluation2 = EvaluateHand(hand2);

                if (evaluation1.rank > evaluation2.rank) return 1;
                if (evaluation1.rank < evaluation2.rank) return -1;

                // If same rank, compare kickers
                for (int i = 0; i < evaluation1.kickers.Count; i++)
                {
                    if (evaluation1.kickers[i] > evaluation2.kickers[i]) return 1;
                    if (evaluation1.kickers[i] < evaluation2.kickers[i]) return -1;
                }

                return 0; // Exactly equal
            }
        }
        public class TexasHoldEmGame
        {
            public CardsDeck Deck { get; private set; }
            public List<PlayingCard> CommunityCards { get; private set; }
            public List<List<PlayingCard>> PlayerHoleCards { get; private set; }
            public int CurrentPlayer { get; private set; }
            public int DealerPosition { get; private set; }
            public int SmallBlind { get; set; } = 10;
            public int BigBlind { get; set; } = 20;
            public List<int> PlayerChips { get; private set; }
            public List<int> CurrentBets { get; private set; }
            public int Pot { get; private set; }
            public GameStage Stage { get; private set; }

            public Dictionary<int, int> m_PokerIDToPlayerID = new Dictionary<int, int>();


            private int s_StartingChips = 0;

            private Server s_Server;
            private string s_GUID;
            private string s_Scene;

            public enum GameStage
            {
                PreFlop,
                Flop,
                Turn,
                River,
                Showdown,
                GameOver
            }

            public TexasHoldEmGame(Server Server, string GUID, string Scene, int numPlayers, int startingChips)
            {
                s_Server = Server;
                s_GUID = GUID;
                s_Scene = Scene;

                if (numPlayers < 2 || numPlayers > 10)
                    throw new ArgumentException("Number of players must be between 2 and 10");

                Deck = new CardsDeck();
                CommunityCards = new List<PlayingCard>();
                PlayerHoleCards = new List<List<PlayingCard>>();
                PlayerChips = new List<int>();
                CurrentBets = new List<int>();

                s_StartingChips = startingChips;

                for (int i = 0; i < numPlayers; i++)
                {
                    PlayerHoleCards.Add(new List<PlayingCard>());
                    PlayerChips.Add(0);
                    CurrentBets.Add(0);
                }

                DealerPosition = -1;
                CurrentPlayer = -1;
                Stage = GameStage.PreFlop;
            }

            public void StartGame()
            {
                // Validate there are enough active players
                if (PlayerChips.Count(p => p > 0) < 2)
                {
                    throw new InvalidOperationException("Need at least 2 active players to start the game");
                }

                // Find first active dealer position (skipping players with no chips)
                DealerPosition = FindNextActivePlayer(-1); // Start searching from before position 0

                // Small blind is next active player after dealer
                CurrentPlayer = FindNextActivePlayer(DealerPosition);

                StartNewHand();
            }

            public void StartNewHand()
            {
                // Reset for new hand
                Deck.PopulateCards();
                Deck.ShuffleDeck();
                CommunityCards.Clear();
                Pot = 0;
                CurrentBets = CurrentBets.Select(x => 0).ToList();

                // Deal hole cards only to active players
                for (int i = 0; i < PlayerHoleCards.Count; i++)
                {
                    if (IsPlayerActive(i)) // Only deal to active players
                    {
                        PlayerHoleCards[i].Clear();
                        PlayerHoleCards[i].Add(Deck.m_Cards[0]);
                        Deck.m_Cards.RemoveAt(0);
                        PlayerHoleCards[i].Add(Deck.m_Cards[0]);
                        Deck.m_Cards.RemoveAt(0);
                    }
                }

                // Find next active players for blinds
                int smallBlindPlayer = FindNextActivePlayer(DealerPosition);
                int bigBlindPlayer = FindNextActivePlayer(smallBlindPlayer);

                // Post blinds
                PlayerChips[smallBlindPlayer] -= SmallBlind;
                CurrentBets[smallBlindPlayer] = SmallBlind;

                PlayerChips[bigBlindPlayer] -= BigBlind;
                CurrentBets[bigBlindPlayer] = BigBlind;

                Pot = SmallBlind + BigBlind;
                Stage = GameStage.PreFlop;
                CurrentPlayer = FindNextActivePlayer(bigBlindPlayer); // Action starts after big blind
            }

            private bool IsPlayerActive(int playerIndex)
            {
                // Player is active if they have chips and haven't folded
                return PlayerChips[playerIndex] > 0 && m_PokerIDToPlayerID.ContainsKey(playerIndex);
            }

            public void EliminatePlayer(int playerIndex)
            {
                PlayerChips[playerIndex] = 0;
                PlayerHoleCards[playerIndex].Clear();
                if (m_PokerIDToPlayerID != null)
                    m_PokerIDToPlayerID.Remove(playerIndex);
            }

            private int FindNextActivePlayer(int startingPosition)
            {
                int nextPlayer = (startingPosition + 1) % PlayerHoleCards.Count;
                while (!IsPlayerActive(nextPlayer))
                {
                    nextPlayer = (nextPlayer + 1) % PlayerHoleCards.Count;

                    // Prevent infinite loop if no active players (shouldn't happen)
                    if (nextPlayer == startingPosition)
                        throw new InvalidOperationException("No active players found");
                }
                return nextPlayer;
            }

            public void ProceedToNextStage()
            {
                switch (Stage)
                {
                    case GameStage.PreFlop:
                        // Deal flop
                        Deck.m_Cards.RemoveAt(0); // Burn card
                        for (int i = 0; i < 3; i++)
                        {
                            CommunityCards.Add(Deck.m_Cards[0]);
                            Deck.m_Cards.RemoveAt(0);
                        }
                        Stage = GameStage.Flop;
                        break;

                    case GameStage.Flop:
                        // Deal turn
                        Deck.m_Cards.RemoveAt(0); // Burn card
                        CommunityCards.Add(Deck.m_Cards[0]);
                        Deck.m_Cards.RemoveAt(0);
                        Stage = GameStage.Turn;
                        break;

                    case GameStage.Turn:
                        // Deal river
                        Deck.m_Cards.RemoveAt(0); // Burn card
                        CommunityCards.Add(Deck.m_Cards[0]);
                        Deck.m_Cards.RemoveAt(0);
                        Stage = GameStage.River;
                        break;

                    case GameStage.River:
                        Stage = GameStage.Showdown;
                        DetermineWinner();
                        break;

                    case GameStage.Showdown:
                        Stage = GameStage.GameOver;
                        break;
                }

                SkyCoopServer.Logger.Log($"ProceedToNextStage NewStage {Stage}");

                if (Stage == GameStage.GameOver)
                {
                    StartNewHand();
                }

                // Reset betting for new stage
                //CurrentBets = CurrentBets.Select(x => 0).ToList();
                //CurrentPlayer = (DealerPosition + 1) % PlayerHoleCards.Count;
            }

            private void DetermineWinner()
            {
                List<List<PlayingCard>> playerHands = new List<List<PlayingCard>>();
                for (int i = 0; i < PlayerHoleCards.Count; i++)
                {
                    if (PlayerChips[i] > 0) // Only consider players who haven't folded
                    {
                        var fullHand = PlayerHoleCards[i].Concat(CommunityCards).ToList();
                        playerHands.Add(fullHand);
                    }
                }

                // Evaluate all hands and find winner(s)
                List<int> winners = new List<int>();
                HandRank bestRank = HandRank.HighCard;
                List<CardType> bestKickers = new List<CardType>();

                for (int i = 0; i < playerHands.Count; i++)
                {
                    var evaluation = PokerHandEvaluator.EvaluateHand(playerHands[i]);
                    int comparison = PokerHandEvaluator.CompareHands(playerHands[i],
                        winners.Count > 0 ? playerHands[winners[0]] : playerHands[i]);

                    if (comparison > 0 || winners.Count == 0)
                    {
                        winners.Clear();
                        winners.Add(i);
                        bestRank = evaluation.rank;
                        bestKickers = evaluation.kickers;
                    }
                    else if (comparison == 0)
                    {
                        winners.Add(i);
                    }
                }

                // Distribute pot to winner(s)
                int winAmount = Pot / winners.Count;
                foreach (int winner in winners)
                {
                    PlayerChips[winner] += winAmount;
                }
            }

            public void PlayerAction(int playerIndex, string action, int amount = 0)
            {
                if (playerIndex != CurrentPlayer)
                    throw new InvalidOperationException("Not this player's turn");

                int currentMaxBet = CurrentBets.Max();
                int playerBet = CurrentBets[playerIndex];

                switch (action.ToLower())
                {
                    case "fold":
                        PlayerHoleCards[playerIndex].Clear();
                        break;

                    case "check":
                        // Only valid if no bet to call
                        if (currentMaxBet > playerBet)
                            throw new InvalidOperationException("Cannot check when there's a bet to call");
                        break;

                    case "call":
                        if (currentMaxBet == playerBet)
                            throw new InvalidOperationException("Cannot call when no bet has been made (should check instead)");

                        int callAmount = currentMaxBet - playerBet;
                        if (callAmount > PlayerChips[playerIndex])
                            throw new InvalidOperationException("Not enough chips to call");

                        PlayerChips[playerIndex] -= callAmount;
                        CurrentBets[playerIndex] += callAmount;
                        Pot += callAmount;
                        break;

                    case "raise":
                        if (currentMaxBet == 0)
                            throw new InvalidOperationException("Cannot raise when no bet has been made (should bet instead)");

                        int totalBet = playerBet + amount;
                        if (totalBet <= currentMaxBet)
                            throw new InvalidOperationException("Raise must exceed current highest bet");
                        if (amount > PlayerChips[playerIndex])
                            throw new InvalidOperationException("Not enough chips to raise");

                        PlayerChips[playerIndex] -= amount;
                        CurrentBets[playerIndex] = totalBet;
                        Pot += amount;
                        break;

                    default:
                        throw new ArgumentException("Invalid action");
                }
                MoveToNextPlayer();
                SendEverything();
            }

            public int GetTotalActivePlayers()
            {
                int Players = 0;
                for (int i = 0; i < PlayerChips.Count; i++)
                {
                    if (m_PokerIDToPlayerID.ContainsKey(i))
                    {
                        Players++;
                    }
                }
                return Players;
            }

            public void SendEverything()
            {
                SendCurrentDealer();
                SendCurrentTurnPlayer();
                SendChips();
                SendBets();
                SendCommunityCards();
                SendPlayersCards(Stage == GameStage.Showdown || Stage == GameStage.GameOver);
            }

            public void SetNewPlayer(int PokerID, int PlayerID)
            {
                m_PokerIDToPlayerID.Add(PokerID, PlayerID);
                PlayerChips[PokerID] = s_StartingChips;

                if(Stage == GameStage.PreFlop && GetTotalActivePlayers() == 2)
                {
                    StartGame();
                }
                SendEverything();
            }

            private void MoveToNextPlayer()
            {

                List<int> ActivePlayers = new List<int>();

                for (int i = 0; i < PlayerChips.Count; i++)
                {
                    int TotalChips = PlayerChips[i] + CurrentBets[i];
                    if (TotalChips > 0 && PlayerHoleCards[i].Count > 0)
                    {
                        ActivePlayers.Add(i);
                    }
                }

                bool bettingComplete = true;

                for (int i = 0; i < ActivePlayers.Count; i++)
                {
                    if (CurrentBets[ActivePlayers[i]] != CurrentBets.Max())
                    {
                        bettingComplete = false;
                    }
                }

                int CurrentPlayerIndexOf = ActivePlayers.IndexOf(CurrentPlayer);

                if(ActivePlayers.Count == 0)
                {

                }
                else 
                {
                    if (CurrentPlayerIndexOf < ActivePlayers.Count - 1)
                    {
                        CurrentPlayer = ActivePlayers[CurrentPlayerIndexOf + 1];
                    }
                    else
                    {
                        CurrentPlayer = 0;
                    }
                }

                if (bettingComplete)
                {
                    ProceedToNextStage();
                }
            }

            public void SendCurrentTurnPlayer()
            {
                foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
                {
                    if (s_Server.GetPlayerDataByNetPeer(Peer).m_Scene == s_Scene)
                    {
                        ServerSend.SendCurrentPlayerTurn(Peer, s_GUID, CurrentPlayer);
                    }
                }
            }
            public void SendCurrentDealer()
            {
                foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
                {
                    if (s_Server.GetPlayerDataByNetPeer(Peer).m_Scene == s_Scene)
                    {
                        ServerSend.SendPokerDealer(Peer, s_GUID, DealerPosition);
                    }
                }
            }

            public void SendChips()
            {
                for (int i = 0; i < PlayerChips.Count; i++)
                {
                    foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
                    {
                        if (s_Server.GetPlayerDataByNetPeer(Peer).m_Scene == s_Scene)
                        {
                            ServerSend.SendPokerChips(Peer, s_GUID, i, PlayerChips[i]);
                        }
                    }
                }
            }

            public void SendBets()
            {
                for (int i = 0; i < CurrentBets.Count; i++)
                {
                    foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
                    {
                        if (s_Server.GetPlayerDataByNetPeer(Peer).m_Scene == s_Scene)
                        {
                            ServerSend.SendPokerBet(Peer, s_GUID, i, CurrentBets[i]);
                        }
                    }
                }
            }

            public void SendCommunityCards()
            {
                for (int i = 0; i < 5; i++)
                {
                    bool SendEmptyCard = false;

                    if(CommunityCards.Count-1 < i)
                    {
                        SendEmptyCard = true;
                    }
                    
                    foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
                    {
                        if (s_Server.GetPlayerDataByNetPeer(Peer).m_Scene == s_Scene)
                        {
                            if (SendEmptyCard)
                            {
                                ServerSend.SendPokerCard(Peer, s_GUID, 4, i, -1, -1);
                            }
                            else
                            {
                                PlayingCard Card = CommunityCards[i];
                                ServerSend.SendPokerCard(Peer, s_GUID, 4, i, (int)Card.m_Type, (int)Card.m_Suit);
                            }
                        }
                    }
                }
            }
            public void SendPlayersCards(bool ShowDown = false)
            {
                for (int PlayerID = 0; PlayerID < PlayerHoleCards.Count; PlayerID++)
                {
                    for(int CardID = 0; CardID < 2; CardID++)
                    {
                        bool SendEmptyCard = false;

                        if (PlayerHoleCards[PlayerID].Count - 1 < CardID)
                        {
                            SendEmptyCard = true;
                        }

                        foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
                        {
                            if (s_Server.GetPlayerDataByNetPeer(Peer).m_Scene == s_Scene)
                            {
                                if (SendEmptyCard)
                                {
                                    ServerSend.SendPokerCard(Peer, s_GUID, PlayerID, CardID, -1, -1);
                                }
                                else
                                {
                                    PlayingCard Card = PlayerHoleCards[PlayerID][CardID];

                                    bool ThisIsMyHand = false;

                                    int ClientID = -1;


                                    if (m_PokerIDToPlayerID.TryGetValue(PlayerID, out ClientID))
                                    {
                                        if(ClientID == Peer.Id)
                                        {
                                            ThisIsMyHand = true;
                                        }
                                    }

                                    if (ShowDown || ThisIsMyHand)
                                    {
                                        ServerSend.SendPokerCard(Peer, s_GUID, PlayerID, CardID, (int)Card.m_Type, (int)Card.m_Suit);
                                    }
                                    else
                                    {
                                        ServerSend.SendPokerCard(Peer, s_GUID, PlayerID, CardID, -2, -2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
