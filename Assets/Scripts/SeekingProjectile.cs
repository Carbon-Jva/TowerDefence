/*
* SeekingProjectile.cs
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

public class SeekingProjectile : Projectile
{
  [Header("Refernces")]
  public Transform trans;

  //Private variables
  private Vector3 targetPosition;

  protected override void OnSetup() {}
  
  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    if(targetEnemy != null)
    {
      //Mark enemy's last position:
      targetPosition = targetEnemy.projectileSeekPoint.position;
    }

    //Point towards the target position:
    trans.forward = (targetPosition - trans.position).normalized;

    //Move towards the target position:
    trans.position = Vector3.MoveTowards(trans.position, targetPosition, speed * Time.deltaTime);

    //If we have reached the target position,
    if(trans.position == targetPosition)
    {
      //Damage the enemy if it's still around:
      if(targetEnemy != null)
      {
        targetEnemy.TakeDamage(damage);
      }

      //Destroy the projectile
      Destroy(gameObject);
    }
  }
}
