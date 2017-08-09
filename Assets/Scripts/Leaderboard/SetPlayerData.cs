using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetPlayerData : MonoBehaviour {

    [SerializeField] private Text _rank;
    [SerializeField] private Text _name;
    [SerializeField] private Text _score;

    public void Set(string rank, string name, string score) {
        _rank.text = rank;
        _name.text = name;
        _score.text = score;
    }
}
