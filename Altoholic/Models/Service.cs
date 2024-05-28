using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud;

namespace Altoholic.Models;
public class Service
{
    public Service(
        Func<Character> getLocalPlayer,
        Func<List<Character>> getOthersCharacters)
    {
        GetLocalPlayer = getLocalPlayer;
        GetOthersCharactersList = getOthersCharacters;
    }

    public Func<Character> GetLocalPlayer { get; init; }
    public Func<List<Character>> GetOthersCharactersList { get; init; }

    public Character GetPlayer()
    {
        return GetLocalPlayer.Invoke();
    }
    
    public List<Character> GetOthersCharacters()
    {
        return GetOthersCharactersList.Invoke();
    }
}