using System;
using System.Collections.Generic;

namespace Altoholic.Models
{
    public class Service(
        Func<Character> getLocalPlayer,
        Func<List<Character>> getOthersCharacters,
        Func<List<Blacklist>> blacklistedCharacters)
    {
        public Func<Character> GetLocalPlayer { get; set; } = getLocalPlayer;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = getOthersCharacters;
        public Func<List<Blacklist>> BlacklistedCharacters { get; set; } = blacklistedCharacters;

        public Character GetPlayer()
        {
            return GetLocalPlayer.Invoke();
        }

        /*public void SetPlayer(Character player)
        {
            GetLocalPlayer.DynamicInvoke([player]);
        }*/
        
        public List<Character> GetOthersCharacters()
        {
            return GetOthersCharactersList.Invoke();
        }
        public List<Blacklist> GetBlacklistedCharacters()
        {
            return BlacklistedCharacters.Invoke();
        }

        public void SetBlacklistedCharacter(ulong id)
        {
            BlacklistedCharacters.DynamicInvoke(new Blacklist { CharacterId = id });
        }
    }
}