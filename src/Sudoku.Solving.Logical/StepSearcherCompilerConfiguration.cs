﻿#pragma warning disable format
[assembly: SearcherConfiguration<                        SingleStepSearcher>(SearcherDisplayingLevel.A)]
[assembly: SearcherConfiguration<              LockedCandidatesStepSearcher>(SearcherDisplayingLevel.A)]
[assembly: SearcherConfiguration<                        SubsetStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                    NormalFishStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                TwoStrongLinksStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                   RegularWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                         WWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<              MultiBranchWWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<               UniqueRectangleStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<        AlmostLockedCandidatesStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                      SueDeCoqStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<            SueDeCoq3DimensionStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                    UniqueLoopStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<             ExtendedRectangleStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                EmptyRectangleStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                  UniqueMatrixStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                 UniquePolygonStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<              QiuDeadlyPatternStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<         BivalueUniversalGraveStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<  ReverseBivalueUniversalGraveStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<           UniquenessClueCoverStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<               RwDeadlyPatternStepSearcher>(SearcherDisplayingLevel.B, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.                   HasBugs)]
[assembly: SearcherConfiguration<EmptyRectangleIntersectionPairStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                      FireworkStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<     GurthSymmetricalPlacementStepSearcher>(SearcherDisplayingLevel.A)]
[assembly: SearcherConfiguration<           NonMultipleChainingStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<            AlmostLockedSetsXzStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<        AlmostLockedSetsXyWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<         AlmostLockedSetsWWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                      GuardianStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<                   ComplexFishStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<                BivalueOddagonStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<              ChromaticPatternStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<                  DeathBlossomStepSearcher>(SearcherDisplayingLevel.C, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.                   HasBugs)]
[assembly: SearcherConfiguration<              MultipleChainingStepSearcher>(SearcherDisplayingLevel.C)]
#if ALLOW_DEPRECATED_LOGICAL_STEP_OR_STEP_SEARCHER
[assembly: SearcherConfiguration<     AlternatingInferenceChainStepSearcher>(SearcherDisplayingLevel.C)]
#endif
[assembly: SearcherConfiguration<                   BowmanBingoStepSearcher>(SearcherDisplayingLevel.C, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.                LastResort)]
[assembly: SearcherConfiguration<                      TemplateStepSearcher>(SearcherDisplayingLevel.C, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.                LastResort)]
[assembly: SearcherConfiguration<                PatternOverlayStepSearcher>(SearcherDisplayingLevel.C, EnabledArea = SearcherEnabledArea.Gathering, DisabledReason = SearcherDisabledReason.                LastResort)]
[assembly: SearcherConfiguration<                  JuniorExocetStepSearcher>(SearcherDisplayingLevel.D)]
[assembly: SearcherConfiguration<                  SeniorExocetStepSearcher>(SearcherDisplayingLevel.D, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.DeprecatedOrNotImplemented)]
[assembly: SearcherConfiguration<                    DominoLoopStepSearcher>(SearcherDisplayingLevel.D)]
[assembly: SearcherConfiguration<         MultisectorLockedSetsStepSearcher>(SearcherDisplayingLevel.D)]
[assembly: SearcherConfiguration<      AdvancedMultipleChainingStepSearcher>(SearcherDisplayingLevel.D, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.DeprecatedOrNotImplemented)]
[assembly: SearcherConfiguration<                    BruteForceStepSearcher>(SearcherDisplayingLevel.E, EnabledArea = SearcherEnabledArea.  Default, DisabledReason = SearcherDisabledReason.                LastResort)]
