using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "object/ObjectBase")]
public class ObjectBase : DescriptionSO
{
    [SerializeField]
    private float _exp;         //having exp
    [SerializeField]
    private int _lv;            //current lv
    [SerializeField]
    private float _size;        //my size

    //[SerializeField]
    //private string _name;
    public float Exp { get => _exp; set => _exp = value; }
    public int Lv { get => _lv; set => _lv = value; }
    public float Size { get => _size; set => _size = value; }
}
