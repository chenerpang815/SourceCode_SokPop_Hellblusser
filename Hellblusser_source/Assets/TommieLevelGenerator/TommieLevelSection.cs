using UnityEngine;
using System.Collections.Generic;

public class TommieLevelSection : MonoBehaviour
{
    // base components
    [HideInInspector] public GameObject myGameObject;

    // type
    [Header("type")]
    public TommieLevelGenerator.SectionType mySectionType;

    // sections
    [Header("sections")]
    public List<TommieLevelGenerator.TommieLevelSectionEntry> sectionsMustCreate;
    public List<TommieLevelGenerator.TommieLevelSectionEntry> sectionsCanCreate;

    // state
    List<TommieLevelGenerator.TommieLevelSectionEntry> sectionsStillNeedToCreate;

    // references
    [Header("references")]
    public TommieLevelSectionExits exits;
    public LevelGenerator.Scripts.Bounds bounds;

    void Start ()
    {
        // base components
        myGameObject = gameObject;

        // init
        sectionsStillNeedToCreate = new List<TommieLevelGenerator.TommieLevelSectionEntry>();
        if (sectionsMustCreate != null)
        {
            for (int i = 0; i < sectionsMustCreate.Count; i++)
            {
                sectionsStillNeedToCreate.Add(sectionsMustCreate[i]);
            }
        }

        // create
        if ((sectionsMustCreate != null && sectionsMustCreate.Count > 0) || (sectionsCanCreate != null && sectionsCanCreate.Count > 0))
        {
            CreateSections();
        }

        // store?
        Store();
    }

    public void CreateSections ()
    {
        if ( exits != null && exits.exitTransforms != null && exits.exitTransforms.Length > 0 )
        {
            int exitCount = exits.exitTransforms.Length;
            List<int> indexList = new List<int>();
            for ( int i = 0; i < exitCount; i ++ )
            {
                indexList.Add(i);
            }
            //BasicFunctions.ShuffleGeneration(indexList);
            for ( int i = 0; i < indexList.Count; i ++ )
            {
                Transform curExitTr = exits.exitTransforms[indexList[i]];

                // still need to create something?
                if ( sectionsStillNeedToCreate != null && sectionsStillNeedToCreate.Count > 0 )
                {
                    //int rIndex = Mathf.RoundToInt(TommieRandom.instance.GenerationRandomRange(0f,sectionsStillNeedToCreate.Count - 1,"sections must create pick"));
                    WeightedRandomBag<int> sectionIndexesPossible = new WeightedRandomBag<int>();
                    for ( int ii = 0; ii < sectionsStillNeedToCreate.Count; ii ++ )
                    {
                        sectionIndexesPossible.AddEntry(ii,10f);
                    }
                    int rIndex = sectionIndexesPossible.Choose();

                    TommieLevelGenerator.TommieLevelSectionEntry sectionEntry = sectionsStillNeedToCreate[rIndex];
                    if (AllowedToPlaceSection(sectionEntry.section.mySectionType))
                    {
                        PlaceSection(curExitTr, sectionEntry.section, sectionEntry.section.mySectionType);
                    }
                    else
                    {
                        TommieLevelSection deadEndChosen = LevelGeneratorManager.instance.activeLevelGenerator.allDeadEnds.ChooseGeneration("deadEndType");
                        if (deadEndChosen != null)
                        {
                            PlaceSection(curExitTr, deadEndChosen, deadEndChosen.mySectionType);
                        }
                    }
                    sectionsStillNeedToCreate.Remove(sectionEntry);
                }
                else
                {
                    if (sectionsCanCreate != null && sectionsCanCreate.Count > 0)
                    {
                        bool placedSection = false;
                        int possibleSectionsCount = 0;
                        WeightedRandomBag<TommieLevelSection> possibleSections = new WeightedRandomBag<TommieLevelSection>();
                        for (int ii = 0; ii < sectionsCanCreate.Count; ii++)
                        {
                            TommieLevelGenerator.TommieLevelSectionEntry entryCheck = sectionsCanCreate[ii];

                            if (AllowedToPlaceSection(entryCheck.section.mySectionType))
                            {
                                possibleSections.AddEntry(entryCheck.section, entryCheck.weight);
                                possibleSectionsCount++;
                            }
                        }
                        if (possibleSectionsCount > 0)
                        {
                            TommieLevelSection sectionChosen = possibleSections.ChooseGeneration("sectionType");
                            PlaceSection(curExitTr, sectionChosen,sectionChosen.mySectionType);
                            placedSection = true;
                        }

                        // place dead end?
                        if ( !placedSection )
                        {
                            TommieLevelSection deadEndChosen = LevelGeneratorManager.instance.activeLevelGenerator.allDeadEnds.ChooseGeneration("deadEndType");
                            if ( deadEndChosen != null )
                            {
                                PlaceSection(curExitTr,deadEndChosen,deadEndChosen.mySectionType);
                            }
                        }
                    }
                }
            }
        }
    }

    public void PlaceSection ( Transform _exit, TommieLevelSection _section, TommieLevelGenerator.SectionType _sectionType ) 
    {
        LevelGeneratorManager.instance.activeLevelGenerator.AddSection(_sectionType);
        Instantiate(_section,_exit.transform.position,_exit.transform.rotation,null);
        LevelGeneratorManager.instance.activeLevelGenerator.generatedSectionCounter = 0;
    }

    public bool AllowedToPlaceSection ( TommieLevelGenerator.SectionType _sectionType )
    {
        TommieLevelGenerator activeLevelGenerator = LevelGeneratorManager.instance.activeLevelGenerator;
        if (activeLevelGenerator != null)
        {
            if (activeLevelGenerator.rules != null && activeLevelGenerator.rules.Length > 0)
            {
                for (int i = 0; i < activeLevelGenerator.rules.Length; i++)
                {
                    TommieLevelGenerator.TommieLevelRules ruleCheck = activeLevelGenerator.rules[i];
                    if ( ruleCheck.sectionType == _sectionType)
                    {
                        int countGet;
                        activeLevelGenerator.sectionCount.TryGetValue(_sectionType, out countGet);
                        if ( countGet >= ruleCheck.maxCount )
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public void Store ()
    {
        if ( LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.allSectionObjects != null )
        {
            LevelGeneratorManager.instance.activeLevelGenerator.allSectionObjects.Add(myGameObject);
        }
    }
}
