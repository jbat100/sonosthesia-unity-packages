using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel.Visuals
{
    public class ChannelCountColor : MonoBehaviour
    {
        [SerializeField] private ChannelBase _channel;

        [SerializeField] private List<Color> _colors;

        private Material _material;
        private Color _baseColor;
        private IDisposable _subscription;

        protected virtual void Awake()
        {
            // TODO : consider material extraction strategy enum
            _material = GetComponent<Renderer>().material;
            _baseColor = _material.color;
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _channel.StreamIds.ObserveCountChanged(true).Subscribe(count =>
            {
                Color color  = count > 0 ? _colors[Mathf.Min(count - 1, _colors.Count - 1)] : _baseColor;;
                _material.color = color;
            });
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}