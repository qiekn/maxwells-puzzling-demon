using System;
using UnityEngine;

namespace qiekn.core {

    [Serializable]
    public enum BorderType {
        conductive,
        sticky,
        none,
    }

    [Serializable]
    public class Border {
        public Vector2Int pos;
        public Vector2Int dir;
        public BorderType type;

        public Border(Vector2Int pos_, Vector2Int dir_, BorderType type_ = BorderType.conductive) {
            pos = pos_;
            dir = dir_;
            type = type_;
        }
    }
}
