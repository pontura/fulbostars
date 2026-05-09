using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Progress
{
    public class PorgressBarPieces : MonoBehaviour
    {
        [SerializeField] ProgressBarPiece piece;
        [SerializeField] Transform container;
       
        public void Init(int selectedID, int qty)
        {
            Utils.RemoveAllChildsIn(container);
            for (int a = 0; a < qty; a++)
            {
                ProgressBarPiece p = Instantiate(piece, container);

                ProgressBarPiece.types t;

                if (a == selectedID-1)
                    t = ProgressBarPiece.types.SELECTED;
                else if (a > selectedID-1)
                    t = ProgressBarPiece.types.INACTIVE;
                else
                    t = ProgressBarPiece.types.DONE;

                p.Init(t);
            }
        }
    }
}
