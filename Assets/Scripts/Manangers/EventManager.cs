using System;

public static class EventManager 
    {
        public static event EventIntroDone IntroDone;
        public static event EventNextMessage NextMessage;
        public delegate void EventIntroDone();
        
        public delegate void EventNextMessage();

        public static event EventBoardCreated BoardCreated;
        public delegate void EventBoardCreated();   

        public static event EventGameOver GameOver;
        public delegate void EventGameOver();

        public static event EventPlayerPlayed PlayerPlayed;
        public delegate void EventPlayerPlayed(TurnObject turnObject);

        public static event EventTurnOver TurnOver;
        public delegate void EventTurnOver(); 
        
        public static event EventInsertionPointClicked InsertionPointClicked;
        public delegate void EventInsertionPointClicked(InsertionPosition point);
        
        public static event EventInsertionLiftUpClicked InsertionLiftUpClicked;
        public delegate void EventInsertionLiftUpClicked();        
        
        public static event EventOnStreakHandlingOver OnStreakHandlingOver;
        public delegate void EventOnStreakHandlingOver(Streak streak);
        
        public static event EventOnSymbolBreakingOver OnSymbolBreakingOver;
        public delegate void EventOnSymbolBreakingOver(MOATile tile);        
        
        public static event EventOnSymbolMovementOver OnSymbolMovementOver;
        public delegate void EventOnSymbolMovementOver(Symbol symbol);        
        
        public static event EventOnInsertionMovementOver OnInsertionMovementOver;
        public delegate void EventOnInsertionMovementOver(Symbol symbol);

        public static void OnTurnOver()
        {
            TurnOver?.Invoke();
        }

        public static void OnBoardCreated()
        {
            BoardCreated?.Invoke();
        }

        public static void OnIntroDone()
        {
            IntroDone?.Invoke();
        }
        
        public static void OnGameOver()
        {
            GameOver?.Invoke();
        }

        public static void OnNextMessage()
        {
            NextMessage?.Invoke();
        }

        public static void OnPlayerPlayed(TurnObject turnObject)
        {
            PlayerPlayed?.Invoke(turnObject);
        }

        public static void OnInsertionClicked(InsertionPosition insertionPosition)
        {
            InsertionPointClicked?.Invoke(insertionPosition);
        }

        public static void OnInsertionLiftUpClicked(InsertionLiftUpPosition insertionLiftUpPosition)
        {
            InsertionLiftUpClicked?.Invoke();
        }
        
        public static void OnStreakGraphicFinished(Streak steak)
        {
            //TODO event
        }        
        
        public static void OnAttackGraphicFinished(Streak steak)
        {
            //TODO event
        }

        public static void SymbolBreakingOver(MOATile moaTile)
        {
            OnSymbolBreakingOver?.Invoke(moaTile);
        }
        
        public static void StreakHandlingOver(Streak streak)
        {
            OnStreakHandlingOver?.Invoke(streak);
        }
        public static void SymbolMovementOver(Symbol symbol)
        {
            OnSymbolMovementOver?.Invoke(symbol);
        }

        public static void InsertionOver(Symbol symbol)
        {
            OnInsertionMovementOver?.Invoke(symbol);
        }
    }

