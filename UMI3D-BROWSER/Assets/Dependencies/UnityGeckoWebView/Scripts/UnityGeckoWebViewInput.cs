using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// Manages screen touches for <see cref="UnityGeckoWebView"/>.
    /// </summary>
    public class UnityGeckoWebViewInput : UnityEngine.UI.Selectable, IPointerMoveHandler
    {
        #region Fields

        [SerializeField, Tooltip("Rect Transform associated to webview raw image transform.")]
        private RectTransform rawImageRectTransform;

        [SerializeField]
        private UnityGeckoWebView webView;

        [SerializeField, Tooltip("Simulate a click when a short pointer down is detected ?")]
        private bool simulateClick = false;

        /// <summary>
        /// Event triggered when a a pointer down event is detected. Pointer coordinates in pixels.
        /// </summary>
        public UnityEvent<Vector2> onPointerDown = new();

        /// <summary>
        /// Event triggered when a a pointer up event is detected. Pointer coordinates in pixels.
        /// </summary>
        public UnityEvent<Vector2> onPointerUp = new();

        #region Data

        /// <summary>
        /// World coordinates of raw image corners.
        /// </summary>
        Vector3[] coordinates = new Vector3[4];

        Vector3 up, right;

        /// <summary>
        /// Last time a up trigger was performed.
        /// </summary>
        private float lastUp = 0;

        /// <summary>
        /// Time to consider that a trigger is a click.
        /// </summary>
        private const int clickTime = 200;

        #endregion

        #endregion

        #region Methods

        protected override void Awake()
        {
            Debug.Assert(rawImageRectTransform != null, "RecTransform can not be null");
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerId);
        }

        public async void OnPointerDown(Vector3 worldHitPoint, int pointerId)
        {
            rawImageRectTransform.GetWorldCorners(coordinates);

            up = (coordinates[1] - coordinates[0]);
            right = (coordinates[3] - coordinates[0]);

            Vector3 localPosition = WorldToLocal(worldHitPoint);

            onPointerDown.Invoke(localPosition);

            if (simulateClick)
            {
                await Task.Delay(clickTime);

                if (Time.time - lastUp < clickTime / 1000f)
                {
                    webView.PointerDown(localPosition.x, localPosition.y, pointerId);
                    await Task.Delay(40);
                    webView.PointerUp(localPosition.x, localPosition.y, pointerId);
                }
                else
                {
                    webView.PointerDown(localPosition.x, localPosition.y, pointerId);
                }
            }
            else
            {
                webView.PointerDown(localPosition.x, localPosition.y, pointerId);
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            OnPointerMove(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerId);
        }

        public void OnPointerMove(Vector3 worldHitPoint, int pointerId)
        {
            Vector3 localPosition = WorldToLocal(worldHitPoint);

            webView.PointerMove(localPosition.x, localPosition.y, pointerId);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUp(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerId);
        }

        public void OnPointerUp(Vector3 worldHitPoint, int pointerId)
        {
            Vector3 localPosition = WorldToLocal(worldHitPoint);
            webView.PointerUp(localPosition.x, localPosition.y, pointerId);

            onPointerUp.Invoke(localPosition);

            lastUp = Time.time;
        }

        public void OnClick(Vector3 worldHitPoint, int pointerId)
        {
            Vector3 localPosition = WorldToLocal(worldHitPoint);
            webView.Click(localPosition.x, localPosition.y, pointerId);
        }

        private Vector3 WorldToLocal(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - coordinates[0];

            localPosition.x = Vector3.Dot(localPosition, right.normalized) / right.magnitude * webView.width;
            localPosition.y = Vector3.Dot(localPosition, up.normalized) / up.magnitude * webView.height;

            return localPosition;
        }

        #endregion
    }
}