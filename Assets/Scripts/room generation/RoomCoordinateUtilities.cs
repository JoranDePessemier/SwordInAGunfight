

using UnityEngine;

public static class RoomCoordinateUtilities
{
    public static float DistanceBetweenRooms { get; set; }

    public static Vector2 RoomToWorldCoordinate(Vector2 roomCoordinate)
    {
        return new Vector2(roomCoordinate.x * DistanceBetweenRooms, roomCoordinate.y * DistanceBetweenRooms);
    }

    public static Vector2 WorldToRoomCoordinate(Vector2 worldCoordinate)
    {
        return new Vector2((int)(worldCoordinate.x/DistanceBetweenRooms),(int)(worldCoordinate.y/DistanceBetweenRooms));
    }
}
