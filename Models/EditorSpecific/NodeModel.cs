#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NewGraph {
    /// <summary>
    /// Editor only part of our NodeModel.
    /// This houses all getters/setters and additional data we need for editor tooling and nice graph functionality.
    /// This will be completely stripped away, when creating a build!
    /// We need to have this in the runtime assembly, otherwise ScriptableObject serialization won't work.
    /// See NodeModel.cs for everything that will be shipped on runtime.
    /// </summary>
    public partial class NodeModel {
        [SerializeField]
        private float nodeX, nodeY;
        [SerializeField]
        private string name;
        [NonSerialized]
        private bool dataIsSet = false;
        public bool isUtilityNode = false;
        public const string nameIdentifier = nameof(name);
        [NonSerialized]
        private SerializedProperty serializedProperty;
        [NonSerialized]
        private SerializedProperty nodeDataSerializedProperty;
        [NonSerialized]
        private SerializedProperty nameProperty;
        [NonSerialized]
        private SerializedProperty nodeXProperty;
        [NonSerialized]
        private SerializedProperty nodeYProperty;
        [NonSerialized]
        private static Dictionary<Type, NodeAttribute> nodeInfo = new Dictionary<Type, NodeAttribute>();
        [NonSerialized]
        public Type nodeType;
        [NonSerialized]
        public NodeAttribute nodeAttribute;

        public NodeModel(INode nodeData) {
            this.nodeData = nodeData;
            Initialize();
        }

        public Vector2 GetPosition() {
            return new Vector2(nodeX, nodeY);
        }

        public string GetName() {
            return name;
        }

        public static NodeAttribute GetNodeAttribute(Type type) {
            if (!nodeInfo.ContainsKey(type)) {
                nodeInfo.Add(type, Attribute.GetCustomAttribute(type, typeof(NodeAttribute)) as NodeAttribute);
            }
            return nodeInfo[type];
        }

        public void Initialize() {
            nodeType = nodeData.GetType();
            nodeAttribute = GetNodeAttribute(nodeType);
        }

        public string SetName(string name) {
            this.name = name;
            if (dataIsSet) {
                nameProperty.serializedObject.Update();
            }
            return name;
        }

        public void SetPosition(float positionX, float positionY) {
            if (dataIsSet) {
                if (positionX != nodeX || positionY != nodeY) {
                    nodeXProperty.floatValue = positionX;
                    nodeYProperty.floatValue = positionY;
                    nodeXProperty.serializedObject.ApplyModifiedProperties();
                }
            } else {
                nodeX = positionX;
                nodeY = positionY;
            }
        }

        public SerializedProperty GetSerializedProperty() {
            return serializedProperty;
        }

        public SerializedProperty GetSpecificSerializedProperty() {
            return nodeDataSerializedProperty;
        }

        public SerializedProperty GetNameSerializedProperty() {
            return nameProperty;
        }

        public void SetData(SerializedProperty serializedProperty) {
            this.serializedProperty = serializedProperty;
            nodeDataSerializedProperty = serializedProperty.FindPropertyRelative(nameof(nodeData));
            nameProperty = serializedProperty.FindPropertyRelative(nameof(name));
            nodeXProperty = serializedProperty.FindPropertyRelative(nameof(nodeX));
            nodeYProperty = serializedProperty.FindPropertyRelative(nameof(nodeY));
            dataIsSet = true;
        }
    }

}
#endif