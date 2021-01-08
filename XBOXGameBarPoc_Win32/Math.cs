namespace XBOXGameBarPoC_Win32
{
    struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    struct Matrix
    {
        float M11;
        float M12;
        float M13;
        float M14;
        float M21;
        float M22;
        float M23;
        float M24;
        float M31;
        float M32;
        float M33;
        float M34;
        float M41;
        float M42;
        float M43;
        float M44;

        public bool WorldToScreen(Vector3 worldLocation, Vector2 screenSize, out Vector2 screenLocation)
        {
            float w = 0.0f;

            screenLocation.X = M11 * worldLocation.X + M12 * worldLocation.Y + M13 * worldLocation.Z + M14;
            screenLocation.Y = M21 * worldLocation.X + M22 * worldLocation.Y + M23 * worldLocation.Z + M24;

            w = M41 * worldLocation.X + M42 * worldLocation.Y + M43 * worldLocation.Z + M44;

            if (w < 0.01f)
                return false;

            screenLocation.X *= (1.0f / w);
            screenLocation.Y *= (1.0f / w);

            float x = screenSize.X / 2;
            float y = screenSize.Y / 2;

            x += 0.5f * screenLocation.X * screenSize.X + 0.5f;
            y -= 0.5f * screenLocation.Y * screenSize.Y + 0.5f;

            screenLocation.X = x;
            screenLocation.Y = y;

            if (screenLocation.X < 0 || screenLocation.X > screenSize.X || screenLocation.Y < 0 || screenLocation.Y > screenSize.Y)
            {
                return false;
            }

            return true;
        }

        public Vector2 WorldToScreen(Vector3 worldLocation, Vector2 screenSize)
        {
            Vector2 screenLocation = new Vector2();

            float w = 0.0f;

            screenLocation.X = M11 * worldLocation.X + M12 * worldLocation.Y + M13 * worldLocation.Z + M14;
            screenLocation.Y = M21 * worldLocation.X + M22 * worldLocation.Y + M23 * worldLocation.Z + M24;

            w = M41 * worldLocation.X + M42 * worldLocation.Y + M43 * worldLocation.Z + M44;

            if (w < 0.01f)
                return new Vector2(0, 0);

            screenLocation.X *= (1.0f / w);
            screenLocation.Y *= (1.0f / w);

            float x = screenSize.X / 2;
            float y = screenSize.Y / 2;

            x += 0.5f * screenLocation.X * screenSize.X + 0.5f;
            y -= 0.5f * screenLocation.Y * screenSize.Y + 0.5f;

            screenLocation.X = x;
            screenLocation.Y = y;

            return screenLocation;
        }
    }

    struct Matrix3x4
    {
        float M11;
        float M12;
        float M13;
        float M14;
        float M21;
        float M22;
        float M23;
        float M24;
        float M31;
        float M32;
        float M33;
        float M34;

        public Vector3 ToVector3()
        {
            return new Vector3(M14, M24, M34);
        }
    }

    struct Box
    {
        float X;
        float Y;
        float Width;
        float Height;

        public Box(Vector2 headScreenLocation, Vector2 feetScreenLocation)
        {
            Y = headScreenLocation.Y;
            Height = feetScreenLocation.Y - headScreenLocation.Y;
            Width = Height / 2.5f;
            X = headScreenLocation.X - Width / 2;
        }
    }
}
