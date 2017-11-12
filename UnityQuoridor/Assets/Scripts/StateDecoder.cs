using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    static class StateDecoder
    {
        public static void DecodeState(BitArray state, out PlayerInfo[] currentPlayers,  out int currentPlayer, out List<WallPeg> WallPegList)
        {
            WallPegList = new List<WallPeg>();
            int c = 0;
            int numPlayers;
            if (!state[c])
            {
                numPlayers = 2;
                c++;
                if (!state[c])
                {
                    currentPlayer = 0;
                }
                else
                {
                    currentPlayer = 1;
                }
            }

            else
            {
                c++; //corresponds to bit 1
                if (!state[c])
                {
                    numPlayers = 3;
                }
                else
                {
                    numPlayers = 4;
                }
                c++;
                if (!state[c])
                {
                    c++;
                    if (!state[c])
                    {
                        currentPlayer = 0;
                    }
                    else
                    {
                        currentPlayer = 1;
                    }
                }
                else
                {
                    c++;
                    if (!state[c])
                    {
                        currentPlayer = 2;
                    }
                    else
                    {
                        currentPlayer = 3;
                    }
                }
            }
            currentPlayers = new PlayerInfo[numPlayers];
            c++;
            if (state[c])   //There is wall on the board
            {
                c++;
                for (int j = 1; j <= 15; j += 2)
                {
                    for (int i = 1; i <= 15; i += 2)
                    {
                        if (state[c])
                        {
                            c++;
                            if (!state[c])
                            {
                                //collisions[i, j] = 'H';
                            }
                            else
                            {
                                //collisions[i, j] = 'V';
                            }
                        }
                        c++;
                    }
                }
            }
            else
            {
                //c++; //corresponds to bit 1
                //if (!bit[c])
                //{
                //    numPlayers = 3;
                //}
                //else
                //{
                //    numPlayers = 4;
                //}
                //c++;
                //if (!bit[c])
                //{
                //    c++;
                //    if (!bit[c])
                //    {
                //        currentPlayer = 0;
                //    }
                //    else
                //    {
                //        currentPlayer = 1;
                //    }
                //}
                //else
                //{
                //    c++;
                //    if (!bit[c])
                //    {
                //        currentPlayer = 2;
                //    }
                //    else
                //    {
                //        currentPlayer = 3;
                //    }
                //}
            }
            long num = 0;
            int bound = 20;
            long div = 891;
            long div2 = 11;
            if (numPlayers == 3)
            {
                bound = 28;
                div = 567;
                div2 = 7;
            }
            else if (numPlayers == 4)
            {
                bound = 36;
                div = 486;
                div2 = 6;
            }
            for (int i = 0; i < bound; i++)
            {
                num *= 2;
                if (state[c])
                {
                    num++;
                }
                c++;
            }
            long rem = 0;
            for (int j = 0; j < numPlayers; j++)
            {
                num = Math.DivRem(num, div, out rem);
                long temp;
                rem = Math.DivRem(rem, div2, out temp);
                currentPlayers[j].wallsLeft = (int)temp;
                rem = Math.DivRem(rem, 9, out temp);
            
                currentPlayers[j].x = 2 * (int)temp;
                currentPlayers[j].y = 2 * (int)rem;
            }
        }
        //internal static List<BitArray> GetAllPossibleMoves(BitArray state)
        //{
        //    PlayerInfo[] currentPlayers;
        //    List<WallPeg> wallPegList = new List<WallPeg>();
        //    int currentPlayer;
        //    DecodeState(out currentPlayers, state, out currentPlayer, ref wallPegList);
            
        //    //Need algorithm to generate possible moves from current state


        //    return null;
        //}
        public static PlayerInfo[] DecodePlayerInfo(BitArray state)
        {
            throw new NotImplementedException();
        }
    }
}
