
    /// <summary>
    /// character当前状态
    /// </summary>
    public struct CharacterResource
    {
        public int hp;
        public int critCountDown;

        public CharacterResource(int hp,int critCountDown)
        {
            this.hp = hp;
            this.critCountDown = critCountDown;
        }
    }
