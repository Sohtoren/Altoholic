[1mdiff --git a/Altoholic/Plugin.cs b/Altoholic/Plugin.cs[m
[1mindex ea57444..d735582 100644[m
[1m--- a/Altoholic/Plugin.cs[m
[1m+++ b/Altoholic/Plugin.cs[m
[36m@@ -774,6 +774,10 @@[m [mnamespace Altoholic[m
             GetHousing();[m
             GetPlayerGlamourInventory();[m
 [m
[32m+[m[32m            if (_autoSaveWatch.Elapsed is { Minutes: 1 or 2 or 3 or 4, Seconds: 0 })[m[41m[m
[32m+[m[32m            {[m[41m[m
[32m+[m[32m                GetCollectionFromState();[m[41m[m
[32m+[m[32m            }[m[41m[m
             if (_autoSaveWatch.Elapsed.Minutes < Configuration.AutoSaveTimer)[m
             {[m
                 return;[m
[36m@@ -1112,8 +1116,8 @@[m [mnamespace Altoholic[m
 [m
                 byte rank = player.GetBeastTribeRank((byte)i);[m
                 ushort val = player.GetBeastTribeCurrentReputation((byte)i);[m
[31m-[m
                 BeastTribeRank? b = _localPlayer.BeastReputations.Find(br => br.Id == i);[m
[32m+[m[41m                [m
                 if (b == null)[m
                 {[m
                     _localPlayer.BeastReputations.Add(new BeastTribeRank[m
[1mdiff --git a/Altoholic/Windows/ProgressWindow.cs b/Altoholic/Windows/ProgressWindow.cs[m
[1mindex a1b1f16..13ad757 100644[m
[1m--- a/Altoholic/Windows/ProgressWindow.cs[m
[1m+++ b/Altoholic/Windows/ProgressWindow.cs[m
[36m@@ -5106,16 +5106,16 @@[m [mnamespace Altoholic.Windows[m
 [m
                             if (rank >= 6)[m
                             {[m
[31m-                                ImGui.TableSetColumnIndex(3);[m
[31m-                                //DrawOrnament(44, currentCharacter.HasOrnament(44));[m
[32m+[m[32m                                ImGui.TableSetColumnIndex(2);[m
[32m+[m[32m                                DrawMinion(545, currentCharacter.HasMinion(545));[m
                             }[m
 [m
                             if (rank >= 7)[m
                             {[m
[31m-                                ImGui.TableSetColumnIndex(4);[m
[31m-                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(48085);[m
[32m+[m[32m                                ImGui.TableSetColumnIndex(3);[m
[32m+[m[32m                                /*uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(48085);[m
                                 if (fkId == null) return;[m
[31m-                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));[m
[32m+[m[32m                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));*/[m
                             }[m
                         }[m
 [m
