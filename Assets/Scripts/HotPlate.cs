/*
* HotPlate.cs
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

public class HotPlate : TargetingTower
{
  public float damagePerSecond = 10;


  // Start is called before the first frame update
  /*void Start()
  {
      
  }*/

  // Update is called once per frame
  void Update()
  {
    //If we have any targets:
    if(targeter.TargetsAreAvailable)
    {
      //Loop through them:
      for(int i = 0; i < targeter.enemies.Count; i++)
      {
        Enemy enemy = targeter.enemies[i];

        //Only burn ground enemies:
        if(enemy is GroundEnemy)
        {
          enemy.TakeDamage(damagePerSecond * Time.deltaTime);
        }
      }
    }
  }
}
