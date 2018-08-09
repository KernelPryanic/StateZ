using GTA;

namespace StateZ.Static
{
    public class Relationships
    {
        public static int InfectedRelationship;

        public static int FriendlyRelationship;

        public static int MilitiaRelationship;

        public static int HostileRelationship;

        public static int PlayerRelationship;

        static Relationships()
        {
        }

        public static void SetRelationships()
        {
            InfectedRelationship = World.AddRelationshipGroup("Zombie");
            FriendlyRelationship = World.AddRelationshipGroup("Friendly");
            MilitiaRelationship = World.AddRelationshipGroup("Private_Militia");
            HostileRelationship = World.AddRelationshipGroup("Hostile");
            PlayerRelationship = Database.PlayerPed.RelationshipGroup;
            SetRelationshipBothWays(GTA.Relationship.Hate, InfectedRelationship, FriendlyRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, InfectedRelationship, MilitiaRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, InfectedRelationship, HostileRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, InfectedRelationship, PlayerRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, FriendlyRelationship, MilitiaRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, FriendlyRelationship, HostileRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, HostileRelationship, MilitiaRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, HostileRelationship, PlayerRelationship);
            SetRelationshipBothWays(GTA.Relationship.Hate, PlayerRelationship, MilitiaRelationship);
            SetRelationshipBothWays(GTA.Relationship.Like, PlayerRelationship, FriendlyRelationship);
            Database.PlayerPed.IsPriorityTargetForEnemies = true;
        }

        public static void SetRelationshipBothWays(Relationship rel, int group1, int group2)
        {
            World.SetRelationshipBetweenGroups(rel, group1, group2);
            World.SetRelationshipBetweenGroups(rel, group2, group1);
        }
    }
}
