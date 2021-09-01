using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InterfaceSet
{
    // 공격 관련 인터페이스
    public interface IAttack
    {
        // 공격을 할 때 시행될 함수
        public void Attack();
    } 

    // 피해를 받는 인터페이스
    public interface IDamaged
    {
        // 피해를 받았을 때 시행될 함수
        public void Damaged(float damage,Vector3 hitPoint, Vector3 hitNormal);
    }


}
