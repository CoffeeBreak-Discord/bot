﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class GiveawayConfig
{
    public ulong ID { get; set; } = 0;
    public ulong GuildID { get; set; } = 0;
    public ulong ChannelID { get; set; } = 0;

    public static void Relations(ModelBuilder model)
    {
        model.Entity<GiveawayConfig>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.GuildID).IsRequired();
            entity.Property(e => e.ChannelID).IsRequired();
        });
    }
}
