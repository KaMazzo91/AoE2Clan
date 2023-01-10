using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoE2Clan.DB
{
    class ClanInfo
    {
        public int Id { get; set; }
        public long GroupId { get; set; }
        public string Clan { get; set; }

        public void insertClanInfo(string ClanName, long Id)
        {
            using (var db = new MyDbContext())
            {
                if (db.clanInfos.Where(s => s.GroupId == Id).Count() > 0)
                {
                    ClanInfo clanInfoToChange = db.clanInfos.Where(s => s.GroupId == Id).FirstOrDefault();
                    if (clanInfoToChange != null)
                    {
                        clanInfoToChange.Clan = ClanName;
                        db.SaveChanges();
                    }
                }
                else
                {
                    ClanInfo clanInfo = new ClanInfo();
                    clanInfo.Clan = ClanName;
                    clanInfo.GroupId = Id;
                    db.clanInfos.Add(clanInfo);
                    db.SaveChanges();
                }
            }
        }

        public string GetClan(long id)
        {
            using (var db = new MyDbContext())
            {
                if (db.clanInfos.Where(s => s.GroupId == id).Any())
                {
                    return db.clanInfos.Where(s => s.GroupId == id).FirstOrDefault()?.Clan;
                }
                else
                {
                    return "";
                }
            }
        }

        public void DbSetup()
        {
            using (var db = new MyDbContext())
            {
                db.Database.EnsureCreated();
                db.Database.Migrate();
            }
        }
    }
}
