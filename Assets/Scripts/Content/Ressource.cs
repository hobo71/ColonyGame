
[System.Serializable]
public class ressourceStack {
    private float amount;
    private readonly ressources type;

    public ressourceStack clone() {
        return new ressourceStack(amount, type);
    }

    public ressourceStack(float amount, ressources type) {
        this.amount = amount;
        this.type = type;
    }

    public float getAmount() {
        return amount;
    }

    public ressources getRessource() {
        return type;
    }

    public void addAmount(float amount) {
        this.amount += amount;
    }

    public override string ToString() {
        return type + ": " + amount;
    }

    public void setAmount(float amount) {
        this.amount = amount;
    }

    public string getNice() {
        return type.ToString() + ": " + (int) amount;
    }

    public static string getNice(ressourceStack[] stacks) {
        string toReturn = "";

        bool firstElem = false;
        foreach (ressourceStack elem in stacks) {
            if (firstElem) {
                toReturn += ", ";
            } else {
                firstElem = true;
            }

            toReturn += elem.getNice();
        }

        return toReturn;
    }

    public static float[] getFloats(ressourceStack[] stacks) {
        float[] floats = new float[stacks.Length];
        for(int i = 0; i < stacks.Length; i++) {
            floats[i] = stacks[i].getAmount();
        }

        return floats;
    }
}

//edit in editor: ressource handler list length (both ressource display and ressource handler) in terrain + ui elem
//also, add 2 entries in the resourceDisplay.cs script

public enum ressources {Wood, Stone, Scrap, Trees, Iron, OreIron, Gold, OreGold, Iridium, OreIridium, Water, Electrum, Uranium, Thorium};