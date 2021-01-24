using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Class that manages a group responsible to lead 
/// pieces until a possible bottom position
/// </summary>
public class Group : MonoBehaviour
{
    [HideInInspector]
    public float verticalSpeed = 2f;
    public List<Piece> pieces = new List<Piece>();
    public Color[] possibleColors;
    [HideInInspector]
    public Spawner spawner;
    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip releasePiecesSfx;
    public AudioClip moveSfx;

    private bool canMove = true;
    private float movementTimer = 0f;

    /// <summary>
    /// Class that returns information about a current 
    /// position of a group that has jus moved
    /// </summary>
    struct MovementInfo
    {
        public bool hasFoundGround;
        public bool hasFoundWall;
        public bool hasFoundSidePiece;
        public bool hasFoundUnderPiece;

        public MovementInfo(bool froundGround, bool foundPiece, bool foundSidePiece, bool foundWall)
        {
            hasFoundGround = froundGround;
            hasFoundUnderPiece = foundPiece;
            hasFoundWall = foundWall;
            hasFoundSidePiece = foundSidePiece;
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        verticalSpeed = LevelManager.instance.gameSpeed;
        GetPieces();
        SetRandomRotation();
        Color color = SortColor();
        ApplyColorToPieces(color);
    }

    void Update()
    {
        if (!LevelManager.instance.isGameOver && !LevelManager.instance.isGamePaused) { 
            movementTimer += Time.deltaTime;
            if (movementTimer >= (1 / verticalSpeed))
            {
                movementTimer = 0;
                if(canMove)
                    Move(0, -1);
            }
            ReadInputs();
            //DEBUG
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
                ReleasePieces();
            #endif
        }
    }

    /// <summary>
    /// Get all child pieces of the group
    /// </summary>
    private void GetPieces()
    {
        foreach (Transform child in transform)
        {
            pieces.Add(child.gameObject.GetComponent<Piece>());
        }
    }

    /// <summary>
    /// Set a random rotation to the group
    /// </summary>
    private void SetRandomRotation()
    {
        float[] angles = new float[4] { 0, 90, 180, 270 };
        TryToRotate(angles[Random.Range(0, angles.Length)]);
    }

    /// <summary>
    /// Tries to rotate the group and checks if the group
    /// is in a valid position. If not, undo the rotation
    /// </summary>
    /// <param name="angle">Desired rotation (ideally multiple of 90)</param>
    public void TryToRotate(float angle)
    {
        audioSource.PlayOneShot(moveSfx);
        Vector3 rotationPoint = Vector3.zero;
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), angle);

        MovementInfo movementInfo = IsPositionValid(Vector3.negativeInfinity);

        if (movementInfo.hasFoundUnderPiece || movementInfo.hasFoundGround || movementInfo.hasFoundSidePiece)
        {
            if(movementInfo.hasFoundSidePiece)
            {
                canMove = false;
                ReleasePieces();
            }
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -angle);
            return;
        }
        if(movementInfo.hasFoundWall)
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -angle);
    }

    /// <summary>
    /// Apply a color to all pieces of the group
    /// </summary>
    /// <param name="color">Desired color</param>
    private void ApplyColorToPieces(Color color)
    {
        if (pieces != null && pieces.Count > 0)
        {
            foreach (Piece piece in pieces)
            {
                piece.SetColor(color);
            }
        }
    }

    /// <summary>
    /// Returns a random color between the array [possibleColors]
    /// </summary>
    /// <returns>Color</returns>
    private Color SortColor()
    {
        if (possibleColors != null && possibleColors.Length > 0)
            return possibleColors[Random.Range(0, possibleColors.Length)];
        return Color.white;
    }

    /// <summary>
    /// Reads player inputs
    /// </summary>
    private void ReadInputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(-1, 0);
            PlaySfx(moveSfx);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(1, 0);
            PlaySfx(moveSfx);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(0, -1);
            PlaySfx(moveSfx);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryToRotate(90);
            PlaySfx(moveSfx);
        }
    }

    /// <summary>
    /// Moves the group to a desired direction.
    /// Undo the movement if the result position is not valid
    /// </summary>
    /// <param name="dx">Unit vector representing x direction</param>
    /// <param name="dy">Unit vector representing y direction</param>
    public void Move(float dx, float dy)
    {
        transform.position += new Vector3(dx, dy, 0);
        MovementInfo movementInfo = IsPositionValid(new Vector3(dx, dy, 0));
        if (movementInfo.hasFoundUnderPiece || movementInfo.hasFoundGround || movementInfo.hasFoundSidePiece)            
        {
            transform.position += new Vector3(-dx, -dy, 0);
            if (!movementInfo.hasFoundSidePiece)
            {
                canMove = false;
                ReleasePieces();
            }
            return;
        }
        if (movementInfo.hasFoundWall)
            transform.position += new Vector3(-dx, -dy, 0);
    }

    /// <summary>
    /// Checks whether the current group position is valid
    /// </summary>
    /// <returns>MovementInfo class containing detailed info</returns>
    private MovementInfo IsPositionValid(Vector3 lastMovementDirection)
    {
        MovementInfo movementInfo = new MovementInfo(false, false, false, false);
        foreach (Piece piece in pieces)
        {
            int x = Mathf.RoundToInt(piece.transform.position.x);
            int y = Mathf.RoundToInt(piece.transform.position.y);

            if (x < 0 || x >= Board.instance.boardWidth)
                movementInfo.hasFoundWall = true;

            if (y < 0)
                movementInfo.hasFoundGround = true;

            try
            {
                if (Board.instance.board[x, y] != null) {
                    float pieceLastLine = piece.transform.position.y - lastMovementDirection.y;
                    float existingPieceLine = Board.instance.board[x, y].transform.position.y;

                    if (pieceLastLine > existingPieceLine)
                    {
                        movementInfo.hasFoundUnderPiece = true;
                        return movementInfo;
                    }
                    movementInfo.hasFoundSidePiece = true;
                }
            } catch(IndexOutOfRangeException ex)
            {
                #if UNITY_EDITOR
                //Debug.LogFormat("Exception lançada ao tentar verificar a piece em LevelManager.instance.grid[{0}, {1}]: \n{2}", x, y, ex);
                #endif
            }
        }
        return movementInfo;
    }

    /// <summary>
    /// Releases (unparent) all child pieces to out of the group,
    /// calls the spawner function to spawn another group and
    /// calls a function from level manager to verify if we
    /// have completed lines in the board
    /// </summary>
    public void ReleasePieces()
    {
        spawner.Spawn();

        if (pieces != null && pieces.Count > 0)
        {
            foreach (Piece piece in pieces)
            {
                int roundedX = Mathf.RoundToInt(piece.transform.position.x);
                int roundedY = Mathf.RoundToInt(piece.transform.position.y);
                piece.transform.SetParent(null);
                piece.transform.position = new Vector3(roundedX, roundedY, piece.transform.position.z);
                Board.instance.board[roundedX, roundedY] = piece;

                if (roundedY >= Board.instance.maxHeight)
                    LevelManager.instance.SetGameOver();
            }
        }
        Board.instance.VerifyAllLines();
        Destroy(gameObject);
    }

    public void PlaySfx(AudioClip sfx)
    {
        if (sfx != null)
            audioSource.PlayOneShot(sfx);
    }
}
