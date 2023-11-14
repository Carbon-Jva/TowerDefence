/*
* GroundEnemy.cs
* Description of the content and purpose of the file.
*
* Copyright (c) 2023 Jimmy Vall
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundEnemy : Enemy
{
  public static NavMeshPath path;

  public float movespeed = 22;

  private int currentCornerIndex = 0;
  private Vector3 currentCorner;


  private bool CurrentCornerIsFinal
  {
    get
    {
      return currentCornerIndex == (path.corners.Length -1);
    }
  }

  private void GetNextCorner()
  {
    //Increment the corner index:
    currentCornerIndex += 1;

    //Set currentCorner to corner with the updated index:
    currentCorner = path.corners[currentCornerIndex];
  }

  // Start is called before the first frame update
  protected override void Start()
  {
    base.Start();
    currentCorner = path.corners[0];
  }

  // Update is called once per frame
  void Update()
  {
    //If this is not the first corner,
    if(currentCornerIndex != 0)
    {
      //Point from our position to the current corner position:
      trans.forward = (currentCorner - trans.position).normalized;
    }

    //Move towards the current corner:
    trans.position = Vector3.MoveTowards(trans.position, currentCorner, movespeed * Time.deltaTime);

    //Whenever we reach a corner,
    if(trans.position == currentCorner)
    {
      //If it's the last corner (positioned at the path goal)
      if(CurrentCornerIsFinal)
      {
        Leak();
      }
      else
      {
        GetNextCorner();
      }
    }
  }
}
