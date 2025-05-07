using inetum.unityUtils.observation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui.inputField
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldPinFormatter : MonoBehaviour
    {
        TMP_InputField _inputField;

        InputFieldModelContainer _modelContainer;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _modelContainer = GetComponentInParent<InputFieldModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<InputFieldNotificationsKeys.InputFieldSet>(),
                (Callback)InputFieldSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void InputFieldSet(Notification notification)
        {
            if (notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldSet.IsPin, out bool isPin, false))
            {
                if (isPin)
                    _inputField.onValueChanged.AddListener(OnValueChanged);
                else
                    _inputField.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(string newValue)
        {
            newValue = newValue.Replace(" ", "");

            if (newValue.Length > 6)
                newValue = newValue.Substring(0, 6);

            if (newValue.Length > 3)
                newValue = newValue.Insert(3, " ");

            _inputField.text = newValue;
            StartCoroutine(SetCaretPosition());
        }

        private IEnumerator SetCaretPosition()
        {
            yield return new WaitForEndOfFrame();
            _inputField.MoveToEndOfLine(false, false);
        }

    }
}
