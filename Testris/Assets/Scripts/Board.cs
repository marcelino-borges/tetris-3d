using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;
    public int boardWidth = 12;
    public int boardHeight = 30;
    public int maxHeight = 22;
    public Piece[,] board;
    public AudioClip completeLineSfx;
    public ParticleSystem pieceExplosion_Particles;

#if UNITY_EDITOR
    public Vector3 highlightPoint = Vector3.zero;
    #endif

    private void Awake()
    {
        if (instance == null)
            instance = this;

        board = new Piece[boardWidth, boardHeight];
        GetComponent<SpriteRenderer>().size = new Vector2(boardWidth, boardHeight);
    }

    public void IncreaseScore()
    {
        LevelManager.instance.IncreaseScore();
    }

    /// <summary>
    /// Calls the co-routine to verify all the lines completeness
    /// </summary>
    public void VerifyAllLines()
    {
        StartCoroutine(VerifyAllLinesCo());
    }

    public IEnumerator VerifyAllLinesCo()
    {
        for (int line = maxHeight - 1; line >= 0; line--)
        {
            if (IsLineComplete(line))
            {
                DeleteLine(line);
                IncreaseScore();
                yield return new WaitForSeconds(.1f);
                MovePiecesDownFromLineToTop(line);
            }
            yield return null;
        }
    }

    private void MovePiecesDownFromLineToTop(int linetoStartFrom)
    {
        for (int line = linetoStartFrom; line < maxHeight; line++)
        {
            for (int column = 0; column < boardWidth; column++)
            {
                if (board[column, line] != null)
                {
                    board[column, line - 1] = board[column, line];
                    board[column, line] = null;
                    board[column, line - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    private bool IsLineComplete(int line)
    {
        for (int column = 0; column < boardWidth; column++)
        {
            if (board[column, line] == null)
                return false;
        }
        return true;
    }

    private void DeleteLine(int line)
    {
        PlaySfx(completeLineSfx);

        for (int column = 0; column < boardWidth; column++)
        {
            if (board[column, line] != null)
            {
                InstantiatePieceExplosionParticles(board[column, line]);

                Destroy(board[column, line].gameObject);
                board[column, line] = null;
            }
        }
    }

    private void InstantiatePieceExplosionParticles(Piece piece)
    {
        if (pieceExplosion_Particles != null)
        {
            ParticleSystem.MainModule particles = Instantiate(
                pieceExplosion_Particles,
                piece.transform.position = new Vector3(piece.transform.position.x, piece.transform.position.y, -3.5f), 
                Quaternion.identity
            ).GetComponent<ParticleSystem>().main;
            particles.startColor = piece.GetComponent<MeshRenderer>().material.color;
        }
    }

    private void PlaySfx(AudioClip sfx)
    {
        SoundManager.instance.PlaySound2D(completeLineSfx);
    }

    //DEBUG
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(x, y, 0), new Vector3(1, 1, 0));
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            new Vector3(
                Mathf.Clamp(highlightPoint.x, 0, boardWidth),
                Mathf.Clamp(highlightPoint.y, 0, boardHeight)
            ),
            new Vector3(1, 1, 0)
        );
    }
    #endif
}

