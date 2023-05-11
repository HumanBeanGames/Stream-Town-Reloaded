using System;
using TechTree.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TechTree.Utilities
{
	public static class TechTreeUtilities
	{
		public static Port CreatePort(this TechTreeNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
		{
			Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

			port.portName = portName;

			return port;
		}

		public static Button CreateButton(string text, Action onClick = null)
		{
			Button button = new Button(onClick)
			{
				text = text
			};

			return button;
		}

		public static Foldout CreateFoldout(string title, bool collapsed = false, EventCallback<ChangeEvent<bool>> onFoldoutChanged = null)
		{
			Foldout foldout = new Foldout()
			{
				text = title,
				value = !collapsed
			};
			
			if(onFoldoutChanged != null)
			{
				foldout.RegisterValueChangedCallback(onFoldoutChanged);
			}

			return foldout;
		}

		public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
		{
			TextField textField = new TextField()
			{
				value = value,
				label = label
			};

			if (onValueChanged != null)
				textField.RegisterValueChangedCallback(onValueChanged);

			return textField;
		}

		public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
		{
			TextField textArea = CreateTextField(value, label, onValueChanged);

			textArea.multiline = true;

			return textArea;
		}

		public static Toggle CreateToggle(bool value, string label = null, EventCallback<ChangeEvent<bool>> onValueChanged = null)
		{
			Toggle toggle = new Toggle()
			{
				value = value,
				label = label
			};

			if (onValueChanged != null)
				toggle.RegisterValueChangedCallback(onValueChanged);

			return toggle;
		}

		public static IntegerField CreateIntField(int value, string label = null, EventCallback<ChangeEvent<int>> onValueChanged = null)
		{
			IntegerField field = new IntegerField()
			{
				value = value,
				label = label
			};

			if (onValueChanged != null)
				field.RegisterValueChangedCallback(onValueChanged);

			return field;
		}

		public static EnumField CreateEnumField(Enum value = null, string label = null, EventCallback<ChangeEvent<Enum>> onValueChanged = null)
		{
			EnumField enumField = new EnumField(label, value);

			if (onValueChanged != null)
				enumField.RegisterValueChangedCallback(onValueChanged);

			return enumField;
		}

		public static FloatField CreateFloatField(float value, string label = null, EventCallback<ChangeEvent<float>> onValueChanged = null)
		{
			FloatField floatField = new FloatField()
			{
				value = value,
				label = label
			};

			if (onValueChanged != null)
				floatField.RegisterValueChangedCallback(onValueChanged);

			return floatField;
		}
	}
}