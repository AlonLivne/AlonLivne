
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    public class TurnObject
    {  
        //the symbol in the turn
        private Symbol _turnSymbol;
        //where the player chose to lay the symbol, chosen position is the index of the move 
        //out of the possible moves array (see class PositionOptions)
        //the direction of the attack
        private (int, int) _coords;
        
        public (int, int) Coords { get => _coords; set => _coords = value; }
        public Symbol TurnSymbol { get => _turnSymbol; set => _turnSymbol = value;}

        private void InsertionPositionClicked(InsertionPosition position)
        {
            _turnSymbol.ReceiveNewParent(position.GetTransform());
        }        
        
        public void Reset()
        {
            EventManager.InsertionPointClicked -= InsertionPositionClicked;
            EventManager.InsertionLiftUpClicked -= InsertionLiftUpClicked;

            _turnSymbol = null;
            _coords = (2, 5);
        }

        public void Activate(InsertionPosition _defaultInsertionPosition, Symbol symbol)
        {
            _turnSymbol = symbol; 
            EventManager.InsertionPointClicked += InsertionPositionClicked;
            EventManager.InsertionLiftUpClicked += InsertionLiftUpClicked;
            EventManager.OnInsertionClicked(_defaultInsertionPosition);
        }

        private void InsertionLiftUpClicked()
        {
            var liftUpCoordinates = InsertionLiftUpPosition.LastHoveredLiftUpPosition.Coordinates;
            var insertionCoords = InsertionPosition.LastHoveredInsertionPosition.Coordinates;
            if (IsPickValidForInsertion(liftUpCoordinates, insertionCoords))
            {
                _coords = insertionCoords;
                InsertSymbol();
                return;
            }
            
            insertionCoords = GetNewInsertionPosition(liftUpCoordinates);
            PickNewInsertionPoint(insertionCoords);
        }

        private bool IsPickValidForInsertion((int, int) liftUpCoordinates, (int, int) insertionPositionCoordinates)
        {
            var ans = false;
            if (liftUpCoordinates.Item2 == 4)//TODO get board size instead of hardcode 4
            {
                ans = ans || IsTopMatching(liftUpCoordinates, insertionPositionCoordinates);
            }

            if (liftUpCoordinates.Item1 == 0 || liftUpCoordinates.Item1 == 4) //TODO get board size instead of hardcode 4
            {
                ans = ans || IsSideMatching(liftUpCoordinates, insertionPositionCoordinates);
            }
            return ans;
        }

        private bool IsSideMatching((int, int) liftUpCoordinates, (int, int) insertionPositionCoordinates)
        {
            return liftUpCoordinates.Item2 == insertionPositionCoordinates.Item2;
        }

        private bool IsTopMatching((int, int) liftUpCoordinates, (int, int) insertionPositionCoordinates)
        {
            return liftUpCoordinates.Item1 == insertionPositionCoordinates.Item1;
        }

        private (int, int) GetNewInsertionPosition((int, int) liftUpCoordinates)
        {
            if (liftUpCoordinates.Item2 == 4) //TODO get board size -1
            {
                if (liftUpCoordinates.Item1 == 0 && liftUpCoordinates.Item1 == 4)
                {
                    if (_coords.Item1 == 0 || _coords.Item1 == 5)
                    {
                        return (_coords.Item1, liftUpCoordinates.Item2);
                    }
                }
                
                return (liftUpCoordinates.Item1, 5);
            }

            if (liftUpCoordinates.Item1 == 0)
            {
                return (-1 , liftUpCoordinates.Item2);
            }

            if (liftUpCoordinates.Item1 == 4)
            {
                return (5 , liftUpCoordinates.Item2);
            }

            return ((int) 0.7, (int) 0.3);

        }

        private void InsertSymbol()
        {
            Debug.Log($"Inserting Symbol from coordinates {_coords}");
            EventManager.OnPlayerPlayed(this);
        }

        private void PickNewInsertionPoint((int,int) coordinates)
        {
            Debug.Log($"Change insertion point to coordinates {coordinates}");
            InsertionPosition.ManuallySelectPosition(coordinates);
        }
    }

