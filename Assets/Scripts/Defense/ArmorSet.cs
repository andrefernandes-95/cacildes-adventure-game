namespace AF
{
    public struct ArmorSet
    {
        public Helmet helmet;
        public Armor armor;
        public Gauntlet gauntlet;
        public Legwear legwear;
        public Accessory accessory1;
        public Accessory accessory2;
        public Accessory accessory3;
        public Accessory accessory4;

        public ArmorBase[] All => new ArmorBase[]
        {
            helmet, armor, gauntlet, legwear, accessory1, accessory2, accessory3, accessory4
        };

        public ArmorSet(Helmet h, Armor a, Gauntlet g, Legwear l, Accessory acc1, Accessory acc2, Accessory acc3, Accessory acc4)
        {
            helmet = h;
            armor = a;
            gauntlet = g;
            legwear = l;
            accessory1 = acc1;
            accessory2 = acc2;
            accessory3 = acc3;
            accessory4 = acc4;
        }

        public void Replace(ArmorBase item, int slot = 0)
        {
            switch (item)
            {
                case Helmet h: helmet = h; break;
                case Armor a: armor = a; break;
                case Gauntlet g: gauntlet = g; break;
                case Legwear l: legwear = l; break;
                case Accessory acc:
                    {
                        if (slot == 0) accessory1 = acc;
                        if (slot == 1) accessory2 = acc;
                        if (slot == 2) accessory3 = acc;
                        if (slot == 3) accessory4 = acc;
                        break;
                    }
            }
        }
    }

}
