using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace JY
{
    public class RoomContents : MonoBehaviour
    {
        [Header("Room Information")]
        public string roomID;
        
        [Header("Room Status")]
        [SerializeField] private bool isRoomUsed = false;
        
        [Header("Room Bounds")]
        public Bounds roomBounds;
        
        [Header("Furniture")]
        private List<FurnitureID> furnitureList = new List<FurnitureID>();
        
        public bool IsRoomUsed => isRoomUsed;
        public int TotalRoomPrice { get; private set; }
        
        private void Start()
        {
            if (string.IsNullOrEmpty(roomID))
            {
                roomID = gameObject.name;
            }
            UpdateRoomContents();
        }
        
        public void SetRoomBounds(Bounds bounds)
        {
            roomBounds = bounds;
            
            // Y축 높이를 5로 조정
            float roomHeight = 4f; // 원하는 Y축 높이
            Vector3 adjustedMin = roomBounds.min;
            Vector3 adjustedMax = roomBounds.max;
            
            float originalYMin = bounds.min.y;
            adjustedMin.y = originalYMin; // 바닥 높이
            adjustedMax.y = roomHeight; // 천장 높이
            
            roomBounds.SetMinMax(adjustedMin, adjustedMax);
            
            UpdateRoomContents();
            Debug.Log($"방 {roomID}의 범위가 업데이트되었습니다. 중심: {bounds.center}, 크기: {bounds.size}");
        }
        
        public void UpdateRoomContents()
        {
            furnitureList.Clear();
            
            // 씬의 모든 FurnitureID 컴포넌트 찾기
            var allFurniture = GameObject.FindObjectsByType<FurnitureID>(FindObjectsSortMode.None);
            
            // roomBounds 안에 있는 가구만 필터링
            foreach (var furniture in allFurniture)
            {
                if (roomBounds.Contains(furniture.transform.position))
                {
                    furnitureList.Add(furniture);
                    Debug.Log($"방 {roomID}에서 가구 발견: {furniture.gameObject.name}, 위치: {furniture.transform.position}");
                }
            }
            
            // 총 가격 계산
            CalculateTotalPrice();
            
            Debug.Log($"방 {roomID} 업데이트: 가구 {furnitureList.Count}개, 총 가격 {TotalRoomPrice}원");
        }
        
        private void CalculateTotalPrice()
        {
            TotalRoomPrice = 0;
            foreach (var furniture in furnitureList)
            {
                if (furniture != null && furniture.Data != null)
                {
                    TotalRoomPrice += furniture.Data.BasePrice;
                    Debug.Log($"가구 가격 추가: {furniture.gameObject.name}, 가격: {furniture.Data.BasePrice}원");
                }
            }
        }
        
        public int UseRoom()
        {
            if (isRoomUsed)
            {
                Debug.LogWarning($"방 {roomID}는 이미 사용 중입니다.");
                return 0;
            }
            
            isRoomUsed = true;
            return TotalRoomPrice;
        }
        
        public void ReleaseRoom()
        {
            isRoomUsed = false;
            Debug.Log($"방 {roomID} 사용 완료");
        }

        private void OnDrawGizmos()
        {
            // 방의 범위를 시각적으로 표시
            Gizmos.color = isRoomUsed ? Color.red : Color.green;
            Gizmos.DrawWireCube(roomBounds.center, roomBounds.size);
        }
    }
} 