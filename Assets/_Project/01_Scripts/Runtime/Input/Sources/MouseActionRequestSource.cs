using System;
using UnityEngine;

namespace Project.InputSystem
{
    public class MouseActionRequestSource : MonoBehaviour, IActionRequestSource
    {
        public event Action<ActionRequest> OnRequest;

        // ✅ 여기 추가: 요청 발행 함수
        public void Raise(ActionRequest request)
        {
            OnRequest?.Invoke(request);
        }
    }
}