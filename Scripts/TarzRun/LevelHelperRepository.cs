using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.UnityObjectRepository;
using Core.Utility;
using UnityEngine;

namespace TarzRun
{
    public static class LevelHelperRepository
    {
        private static readonly UnityObjectRepository<LevelHelper> levelHelperRepository =
                                                    new UnityObjectRepository<LevelHelper>("LEVELS");

        
        public static LevelHelper GetLevel(int level)
        {
            return levelHelperRepository.Items.SingleOrDefault(e => e.levelIndex == level);
        }

        public static LevelHelper GetRandomLevel() => levelHelperRepository.Items.GetRandomItem();

        public static int LevelHelperCount()
        {
            return levelHelperRepository.Items.Count;
        }
        
    }
}
