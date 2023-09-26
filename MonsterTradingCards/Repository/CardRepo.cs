using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MonsterTradingCards.BasicClasses;
using Npgsql;
using NpgsqlTypes;


namespace MonsterTradingCards.Repository;

public class CardRepo : IRepository<Card>
    {
        public Card Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Add(Card t)
        {
            throw new NotImplementedException();
        }

        public void Update(Card t, string[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Delete(Card t)
        {
            throw new NotImplementedException();
        }
    }

