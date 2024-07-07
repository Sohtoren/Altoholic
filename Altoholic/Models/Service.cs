using System;
using System.Collections.Generic;

namespace Altoholic.Models
{
    public class Service(
        Func<Character> getLocalPlayer,
        Func<List<Character>> getOthersCharacters)
    {
        public Func<Character> GetLocalPlayer { get; init; } = getLocalPlayer;
        public Func<List<Character>> GetOthersCharactersList { get; init; } = getOthersCharacters;

        public Character GetPlayer()
        {
            return GetLocalPlayer.Invoke();
        }
    
        public List<Character> GetOthersCharacters()
        {
            return GetOthersCharactersList.Invoke();
        }
    }
}