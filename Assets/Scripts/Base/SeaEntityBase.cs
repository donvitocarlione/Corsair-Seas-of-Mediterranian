using UnityEngine;

namespace CSM.Base
{
    public abstract class SeaEntityBase : MonoBehaviour
    {
        [SerializeField] protected string entityName;
        protected FactionType faction;

        protected virtual void Start()
        {
            if (string.IsNullOrEmpty(entityName))
            {
                entityName = gameObject.name;
            }
        }

        public virtual void SetFaction(FactionType newFaction)
        {
            faction = newFaction;
        }

        public virtual void SetName(string newName)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                entityName = newName;
                gameObject.name = newName;
            }
        }

        public FactionType Faction => faction;
        public string Name => entityName;

        protected virtual void OnDestroy() { }
    }
}