﻿

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PieceMover : MonoBehaviour
{

    [SerializeField] private Square square;
    private Piece piece;
    private PieceBehaviour pieceBehaviour;
    private Camera mainCam;

    [SerializeField] private List<Square> validMoves = new List<Square>();
    public bool HasLocalPermissionToMove { get { return GameLogic.Instance.CurrentPlayer == piece.ColorProperty; } private set { } }

    [SerializeField] private Vector3 origin;
    [SerializeField] private bool isDragging;
    [SerializeField] private Vector2 oldCoordinates;

    private void Start () {

        this.mainCam = Camera.main;
        this.piece = GetComponent<Piece>();
        this.pieceBehaviour = GetComponent<PieceBehaviour>();
    }

    internal void PrepareDragging() {

        if (!Security.GlobalPermission) return;
        if (!isDragging && HasLocalPermissionToMove) {

            this.isDragging = true;
            this.origin = this.transform.position;
            this.oldCoordinates = this.piece.Coordinates;
            this.validMoves.Clear();
            this.validMoves = this.pieceBehaviour.GetValidMoves();
            ApplyIgnoreRaycastLayerToAllPieces();
            EnableVisibilityOfPossibleMoves();
        }
    }
    internal void DragAndRaycast() {

        if (isDragging && HasLocalPermissionToMove) {

            transform.position = GetWorldPosition(transform);
            Debug.DrawRay(transform.position, Vector3.forward * 100);

            if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit)) {

                if (hit.collider.GetComponent<Square>()) {

                    square = hit.collider.GetComponent<Square>();
                }
            } 
            
            else {

                square = null;
            }
        }
    }
    internal void ResolveDragging() {

        while (isDragging) {

            if (square == null) {

                DisableDragging();
                    SendBackToOrigin();
                        ResetRaycastSquare();
                            break;
            }  
            
            else {

                bool thisMoveIsValid = validMoves.Contains(square);

                if (!thisMoveIsValid) { 

                    DisableDragging();
                        SendBackToOrigin();
                            ResetRaycastSquare();
                                break;
                } 
               

                else {

                    // gültiges Feld
                    if (square.CurrentSubscriber == null) {
                      this.piece.CurrentlySubscribedTo.RemoveSubscriber();
                        square.AddSubscriber(this.piece);
                            InCaseOfPawnSetFlagForHasNotMovedYetToFalse(this.piece);
                            InCaseOfPawnCheckIfPawnHasMoved2FieldsAndSetFlagToTrueWhenTheCase(this.piece);
                            InCaseOfPawnCanBeCapturedEnPassantDisableThisProperty();
                                        DisableDragging();
                                            ResetRaycastSquare();
                                                 SetInternalCounter();
                                                    RecruitCheck(this.piece);
                                                        break;
                    }

                    // gegnerische Figur wird geschlagen.
                    else if (square.CurrentSubscriber != null) {

                        this.piece.CurrentlySubscribedTo.RemoveSubscriber();
                        Board.Instance.Pieces.Remove(square.CurrentSubscriber);
                        Destroy(square.CurrentSubscriber.gameObject);
                        square.RemoveSubscriber();
                        square.AddSubscriber(this.piece);

                        InCaseOfPawnSetFlagForHasNotMovedYetToFalse(this.piece);
                        InCaseOfPawnCheckIfPawnHasMoved2FieldsAndSetFlagToTrueWhenTheCase(this.piece);
                        SetInternalCounter();

                        DisableDragging();
                        ResetRaycastSquare();
                        GameLogic.Instance.ChangeActivePlayer();
                        break;
                    }
                }
            }
        }

        DisableVisibilityOfPossibleMoves();
        UnapplyIgnoreRaycastLayerToAllPieces();
    }

    private void ApplyIgnoreRaycastLayerToAllPieces() {

        foreach (var element in Board.Instance.Pieces.Where(x => x != piece)) {

            element.gameObject.layer = 2;
        }
    }
    private void UnapplyIgnoreRaycastLayerToAllPieces() {

        foreach (var element in Board.Instance.Pieces.Where(x => x != piece)) {

            element.gameObject.layer = 10;
        }
    }
    private Vector3 GetWorldPosition(Transform t) {

        var desiredPosition = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.nearClipPlane));
        return desiredPosition;
    }
    private void DisableDragging() {

        isDragging = false;
    }
    private void SendBackToOrigin() {

        this.transform.position = origin;
    }
    private void ResetRaycastSquare () {

        square = null;
    }
    private void EnableVisibilityOfPossibleMoves() {

        if (validMoves.Count > 0) {

            foreach (var sq in validMoves) {

                sq.EnableValidMoveHightlight();
            }
        }
    }
    private void DisableVisibilityOfPossibleMoves() {

        if (validMoves.Count > 0) {

            foreach (var sq in validMoves) {

                sq.DisableValidMoveHighlight();
            }
        }
    }
    private void SetInternalCounter() {

        this.piece.InternalCounter = 0;
        this.piece.InternalCounter += GameLogic.Instance.TurnCounter;
    }

    // Pawn specific methods.
    private void InCaseOfPawnSetFlagForHasNotMovedYetToFalse (Piece piece) {

        if (piece.GetType() == typeof(Pawn)) {

            var pawn = piece as Pawn;
            pawn.hasNotMovedYet = false;
        }
    }
    private void InCaseOfPawnCheckIfPawnHasMoved2FieldsAndSetFlagToTrueWhenTheCase (Piece piece) {

        if (piece.GetType() == typeof(Pawn)) {
            var pawn = this.piece as Pawn;
                if (pawn.hasMoved2Fields) return;
                    if (pawn.Coordinates.y == (this.oldCoordinates.y + 2) || pawn.Coordinates.y == (this.oldCoordinates.y - 2)) {
                        pawn.hasMoved2Fields = true;
                         this.oldCoordinates = Vector2.zero;
            }

            else {
                pawn.hasMoved2Fields = false;
                    this.oldCoordinates = Vector2.zero;
            }
        }
    } // Im Grunde wird für jeden Bauer nur ein einziges Mal geprüft.
    private void InCaseOfPawnCanBeCapturedEnPassantDisableThisProperty() {

        if (this.piece.GetType() == typeof(Pawn)) {
            var pawn = this.piece as Pawn;
                pawn.canBeCapturedEnPassant = false;
        }
    }
    private void RecruitCheck (Piece piece) {

        if (piece.GetType() != typeof(Pawn)) {

            GameLogic.Instance.ChangeActivePlayer();
            return;
        }

        var pawn = piece as Pawn;

        if (pawn.ColorProperty == ColorField.White) {
            if (pawn.Coordinates.y == 8) {
                Security.Lock();
                    GameUIManager.Instance.ShowPieces(gameObject, this.piece);
                         return;
            }

            else GameLogic.Instance.ChangeActivePlayer();
        } 
        
        else if (pawn.ColorProperty == ColorField.Black) {
            if (pawn.Coordinates.y == 1) {
                Security.Lock();
                    GameUIManager.Instance.ShowPieces(gameObject, this.piece);
                        return;
            }

            else GameLogic.Instance.ChangeActivePlayer();
        }

        return;
    }
}