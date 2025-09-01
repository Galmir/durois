using Godot;
using System;
using System.Linq;
using Godot.Collections;

[GlobalClass]
public partial class EquippableItem : InventoryItem {
    [Export] private int sockets;
    [Export] public Array<GemItem> SocketedGems { get; private set; }

    public bool HasFreeSockets() {
        return SocketedGems.Count < sockets;
    }


    public void InsertGem(GemItem gem) {
        if (HasFreeSockets()) {
            SocketedGems.Add(gem);
        }
    }


    public GemItem RemoveGem(int position) {
        if (SocketedGems.ElementAt(position) != null) {
            GemItem removedGem = SocketedGems[position];
            SocketedGems.RemoveAt(position);
            return removedGem;
        }

        return null;
    }

    protected override void UpdateDescription() {
        base.UpdateDescription();
        description += "\n[p]Gem Sockets\n";
        foreach (GemItem gem in SocketedGems) {
            description += "[img]uid://or044ht24m7n[/img] +" + gem.Name + "\n";
        }
        for (int i = 0; i < sockets - SocketedGems.Count(); i++) {
            description += "[img]uid://cmmp6qr4ujuxy[/img] empty \n";
        }
        description += "[/p]";
    }

}

