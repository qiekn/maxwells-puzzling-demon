using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace qiekn.core {
    public class Crate : MonoBehaviour, IMoveable, ITemperature {
        [SerializeField] Temperature temperature;
        public Vector2Int position; // grid position
        public List<Vector2Int> offsets;
        public Dictionary<Vector2Int, Unit> units;
        public List<Border> borders; // outline border

        SpriteRenderer sr;
        GridManager gm;

        void Start() {
            gm = FindFirstObjectByType<GridManager>();
            sr = GetComponent<SpriteRenderer>();
            /*gm.RegisterCrate(this);*/
            UpdateBorders();
            GenerateSprite();
            UpdateColor();
        }

        void UpdateBorders() {
            // generate units
            units = new Dictionary<Vector2Int, Unit>();
            foreach (var offset in offsets) {
                // Debug.Log("units: " + offset.x + " " + offset.y);
                units.Add(offset, new Unit(offset));
            }
            // generate borders
            // and also check neighbors for each unit to disable inner border
            // e.g. if has up direction neighbor, disable up border
            borders = new List<Border>();
            foreach (var pos in offsets) {
                var unit = units[pos];
                foreach (var dir in Defs.directions) {
                    if (units.ContainsKey(pos + dir)) {
                        // Debug.Log("disable border: " + pos + " " + dir);
                        unit.borders[dir].type = BorderType.none;
                    } else {
                        // Debug.Log("enable border: " + pos + " " + dir);
                        borders.Add(unit.borders[dir]);
                    }
                }
            }
            Debug.Log("borders size: " + borders.Count);
        }

        /*─────────────────────────────────────┐
        │               Renderer               │
        └──────────────────────────────────────*/
        public void GenerateSprite() {
            if (sr == null) {
                sr = GetComponent<SpriteRenderer>();
            }

            // calculate sprite bounds
            int width = 0;
            int height = 0;
            foreach (var offset in offsets) {
                width = Mathf.Max(width, offset.x + 1);
                height = Mathf.Max(height, offset.y + 1);
            }

            // create texture
            int texWidth = width * Defs.CellSize;
            int texHeight = height * Defs.CellSize;
            var texture = new Texture2D(texWidth, texHeight);

            // init texture color to transparent
            var pixels = new Color32[texWidth * texHeight];
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = Color.clear;
            }
            texture.SetPixels32(pixels);

            // draw sprite
            foreach (var offset in offsets) {
                var startX = offset.x * Defs.CellSize;
                var startY = offset.y * Defs.CellSize;
                for (int y = startY; y < startY + Defs.CellSize; y++) {
                    for (int x = startX; x < startX + Defs.CellSize; x++) {
                        texture.SetPixel(x, y, Color.white);
                    }
                }
            }

            // draw border
            // to-do

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            var sprite = Sprite.Create(texture, new Rect(0, 0, texWidth, texHeight), new Vector2(0, 0));
            sr.sprite = sprite;
        }

        /*─────────────────────────────────────┐
        │               Movable                │
        └──────────────────────────────────────*/

        /*
        public bool CanMove(Vector2Int direction) {
            var dest = position + direction;
            return gm.CanMoveTo(dest, this);
        }

        public void Move(Vector2Int direction) {
            if (CanMove(direction)) {
                gm.UnRegisterCrate(this);
                position += direction;
                gm.RegisterCrate(this);
                transform.position = new Vector3(position.x, position.y, 0);
            }
        }
        */

        /*─────────────────────────────────────┐
        │             Temperature              │
        └──────────────────────────────────────*/

        public int GetTemperature() {
            return 0;
        }

        public void SetTemperature(int val_) {
            UpdateColor();
            return;
        }

        public void UpdateColor() {
            switch (temperature) {
                case Temperature.Hot:
                    sr.color = Defs.RED;
                    break;
                case Temperature.Cold:
                    sr.color = Defs.BLUE;
                    break;
                case Temperature.Neutral:
                    sr.color = Defs.GRAY;
                    break;
                case Temperature.Magic:
                    sr.color = Defs.GREEN;
                    break;
            }
        }
    }
}
