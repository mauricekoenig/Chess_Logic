﻿

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MauriceKoenig.ChessGame
{
    public sealed class PawnBehaviour : PieceBehaviour
    {
        private Mover mover;
        public override List<Square> GetValidMoves() {

            validSquares.Clear();

            var temp = this.piece.Coordinates;
            var pawn = this.piece as Pawn;

            // ein oder zwei Felder zu Beginn - je nachdem.
            for (int i = 0; i < 2; i++) {

                if (this.piece.ColorProperty == ColorProperty.White) {
                    temp.y++;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();
                    if (newSquare.CurrentSubscriber != null) {
                        if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                        if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) break;
                    } else if (newSquare.CurrentSubscriber == null) {
                        validSquares.Add(newSquare);
                        if (pawn.HasNotMoved) continue; // wenn bereits bewegt, wird dieser Vorgang nicht mehr wiederholt. Ansonsten wird ein weiteres Feld gescannt.
                        else break;
                    }
                }

                // -----------------------------------------------------------------------------------------------------------------------------------

                else if (this.piece.ColorProperty == ColorProperty.Black) {
                    temp.y--;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp).ToList();
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();
                    if (newSquare.CurrentSubscriber != null) {
                        if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                        if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) break;
                    } else if (newSquare.CurrentSubscriber == null) {
                        validSquares.Add(newSquare);
                        if (pawn.HasNotMoved) continue;
                        else break;

                    }
                }
            }

            temp = this.piece.Coordinates;
            // Checke für Gegner: OBEN LINKS.
            for (int i = 0; i < 1; i++) {

                if (this.piece.ColorProperty == ColorProperty.White) {

                    temp.y++; temp.x--;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp).ToList();
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {

                        if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                        if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {

                            validSquares.Add(newSquare);
                            break;
                        }
                    }
                }

                if (this.piece.ColorProperty == ColorProperty.Black) {

                    temp.y--; temp.x++;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp).ToList();
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {

                        if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                        if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {

                            validSquares.Add(newSquare);
                            break;
                        }
                    }
                }
            }

            temp = this.piece.Coordinates;
            // Gegner ermitteln: OBEN RECHTS.
            for (int i = 0; i < 1; i++) {

                if (this.piece.ColorProperty == ColorProperty.White) {

                    temp.y++; temp.x++;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp).ToList();
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {

                        if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                        if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {

                            validSquares.Add(newSquare);
                            break;
                        }
                    }
                }

                if (this.piece.ColorProperty == ColorProperty.Black) {

                    temp.y--; temp.x--;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp).ToList();
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {

                        if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                        if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {

                            validSquares.Add(newSquare);
                            break;
                        }
                    }
                }
            }

            // En Passant Links
            temp = this.piece.Coordinates;
            for (int i = 0; i < 1; i++) {

                if (this.piece.ColorProperty == ColorProperty.White) {

                    temp.x--;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {
                        if (newSquare.CurrentSubscriber.GetType() == typeof(Pawn)) {
                            if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                            if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {
                                Debug.Log("Schwarzer Bauer, der En Passant geschlagen werden kann entdeckt.");
                                Pawn enemyPawn = newSquare.CurrentSubscriber as Pawn;
                                if (PawnIsOnFifthRank() &&
                                    enemyPawn.InternalTurnCounter == GameManager.Instance.GlobalTurnCounter - 1 &&
                                        enemyPawn.HasMovedTwoFields || enemyPawn.CanBeCapturedEnPassant) {
                                    enemyPawn.CanBeCapturedEnPassant = true;
                                    Debug.Log("Dieser weiße Bauer hat alle En Passant Guards überwunden.");
                                    temp.y++;
                                    var enPassantSquare = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                                    if (enPassantSquare.Count() == 0) break;
                                    validSquares.Add(enPassantSquare.Single());
                                }
                            }
                        }
                    }
                } else if (this.piece.ColorProperty == ColorProperty.Black) {

                    temp.x--;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {
                        if (newSquare.CurrentSubscriber.GetType() == typeof(Pawn)) {
                            if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                            if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {
                                Debug.Log("Weißer Bauer, der En Passant geschlagen werden kann entdeckt.");
                                Pawn enemyPawn = newSquare.CurrentSubscriber as Pawn;
                                if (PawnIsOnFifthRank() &&
                                    enemyPawn.InternalTurnCounter == GameManager.Instance.GlobalTurnCounter - 1 &&
                                        enemyPawn.HasMovedTwoFields || enemyPawn.CanBeCapturedEnPassant) {
                                    enemyPawn.CanBeCapturedEnPassant = true;
                                    Debug.Log("Dieser schwarze Bauer hat alle En Passant Guards überwunden.");
                                    temp.y--;
                                    var enPassantSquare = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                                    if (enPassantSquare.Count() == 0) break;
                                    validSquares.Add(enPassantSquare.Single());
                                }
                            }
                        }
                    }
                }
            }

            temp = this.piece.Coordinates;
            // En Passant RECHTS.
            for (int i = 0; i < 1; i++) {

                if (this.piece.ColorProperty == ColorProperty.White) {

                    temp.x++;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {
                        if (newSquare.CurrentSubscriber.GetType() == typeof(Pawn)) {
                            if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                            if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {
                                Debug.Log("Schwarzer Bauer, der En Passant geschlagen werden kann entdeckt.");
                                Pawn enemyPawn = newSquare.CurrentSubscriber as Pawn;
                                if (PawnIsOnFifthRank() &&
                                    enemyPawn.InternalTurnCounter == GameManager.Instance.GlobalTurnCounter - 1 &&
                                        enemyPawn.HasMovedTwoFields || enemyPawn.CanBeCapturedEnPassant) {
                                    enemyPawn.CanBeCapturedEnPassant = true;
                                    Debug.Log("Dieser weiße Bauer hat alle En Passant Guards überwunden.");
                                    temp.y++;
                                    var enPassantSquare = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                                    if (enPassantSquare.Count() == 0) break;
                                    validSquares.Add(enPassantSquare.Single());
                                }
                            }
                        }
                    }
                } else if (this.piece.ColorProperty == ColorProperty.Black) {

                    temp.x++;
                    var next = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                    if (next.Count() == 0) break;
                    var newSquare = next.Single();

                    if (newSquare.CurrentSubscriber != null) {
                        if (newSquare.CurrentSubscriber.GetType() == typeof(Pawn)) {
                            if (newSquare.CurrentSubscriber.ColorProperty == this.piece.ColorProperty) break;
                            if (newSquare.CurrentSubscriber.ColorProperty != this.piece.ColorProperty) {
                                Debug.Log("Weißer Bauer, der En Passant geschlagen werden kann entdeckt.");
                                Pawn enemyPawn = newSquare.CurrentSubscriber as Pawn;
                                if (PawnIsOnFifthRank() &&
                                     enemyPawn.InternalTurnCounter == GameManager.Instance.GlobalTurnCounter - 1 &&
                                        enemyPawn.HasMovedTwoFields || enemyPawn.CanBeCapturedEnPassant) {
                                    enemyPawn.CanBeCapturedEnPassant = true;
                                    Debug.Log("Dieser schwarze Bauer hat alle En Passant Guards überwunden.");
                                    temp.y--;
                                    var enPassantSquare = Board.Instance.Squares.Where(x => x.Coordinates == temp);
                                    if (enPassantSquare.Count() == 0) break;
                                    validSquares.Add(enPassantSquare.Single());
                                }
                            }
                        }
                    }
                }
            }

            return validSquares;
        }
        protected override void Start() {

            base.Start();
        }

        [PawnFlag] private bool PawnIsOnFifthRank() {

            if (this.piece.ColorProperty == ColorProperty.Black) {
                if (this.piece.Coordinates.y == 4) {
                    Debug.Log("Schwarzer Bauer ist auf dem fünften Rang.");
                    return true;
                } else
                    return false;
            } else if (this.piece.ColorProperty == ColorProperty.White) {
                if (this.piece.Coordinates.y == 5) {
                    Debug.Log("Weißer Bauer ist auf dem fünften Rang.");
                    return true;
                } else
                    return false;
            } else
                return false;
        }
    }
}