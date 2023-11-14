/*
* FlyingEnemy.cs
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

public class FlyingEnemy : Enemy
{
  [Tooltip("Units moved per seconds.")]
  public float movespeed;

  private Vector3 targetPosition;


  // Start is called before the first frame update
  protected override void Start()
  {
    base.Start();

    //Set target position to the last corner in the path:
    targetPosition = GroundEnemy.path.corners[GroundEnemy.path.corners.Length - 1];

    //But make the Y poaition equal to the one we were given at start:
    targetPosition.y = trans.position.y;
  }

  // Update is called once per frame
  void Update()
  {
    //Move towards the target position:
    trans.position = Vector3.MoveTowards(trans.position, targetPosition, movespeed * Time.deltaTime);

    //Leak if we've reached the target position:
    if(trans.position == targetPosition)
    {
      Leak();
    }
  }
}
