using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork
{
    #region Data - Model
    public class ContentItemData
    {
        [Tooltip("You can attach your custom behavior")]
        public MonoBehaviour itemBehaviour; 
    }
    #endregion

    #region View
    public class ContentItemBase : MonoBehaviour
    {
        public delegate void OnItemTypeEvent(ContentItemBase item);
    }

    public class ContentItem<DataType> : ContentItemBase where DataType : ContentItemData
    {
        public virtual void Setup(DataType data)
        {

        }
    }
    #endregion

    #region Controller
    public abstract class ContentProviderBase : UIBehaviour
    {
        [Tooltip("Item template")]
        public GameObject template;
        /*
        [Tooltip("Item padding against parent")]
        public Rect padding;
        [Tooltip("Item spacing against each other.")]
        public Vector2 spacing;
        */
        protected int m_NumItems;
        protected Vector2 m_ItemSize;

        public Vector2 itemSize { get { return m_ItemSize; } }

        protected override void Start()
        {
            base.Start();
            Setup();
        }
        
        protected virtual void Setup()
        {
            if (template == null)
            {
                Debug.LogError("Template is never assigned!");
            }

            RectTransform templateRect = template.GetComponent<RectTransform>();
            m_ItemSize = new Vector2(templateRect.rect.width, templateRect.rect.height);
            template.SetActive(false);
        }
    }

    public class ContentProvider<DataType, ItemType> : ContentProviderBase
        where DataType : ContentItemData
        where ItemType : ContentItem<DataType>
    {
        [Tooltip("Data")]
        public DataType[] datas;
        [HideInInspector]
        [Tooltip("Items")]
        public ItemType[] items;
        
        public virtual void SetupDatas(DataType[] datas_)
        {
            ItemType check = template.GetComponent<ItemType>();
            if (check == null)
            {
                Debug.LogError("Template does not have the correct ItemType component!");
            }

            datas = datas_;
            if (items == null || items.Length != datas.Length)
            {
                items = new ItemType[datas.Length];
                for (int i = 0; i < datas.Length; i++)
                {
                    items[i] = NewItem();
                }
            }

            UpdateItems();
        }
        
        /// <summary>
        /// Update whole datas
        /// </summary>
        /// <param name="datas_"></param>
        public virtual void UpdateDatas(DataType[] datas_)
        {
            datas = datas_;
            UpdateItems();
        }

        /// <summary>
        /// Update a single data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="i"></param>
        public virtual void UpdateData(DataType data, int i)
        {
            datas[i] = data;
            items[i].Setup(data);
        }

        protected virtual void UpdateItems()
        {
            for (int i = 0; i < datas.Length; i++)
            {
                items[i].Setup(datas[i]);
            }
        }

        protected virtual ItemType NewItem()
        {
            GameObject instanced = Object.Instantiate(template);
            instanced.transform.SetParent(transform, false);
            instanced.SetActive(true);
            return instanced.GetComponent<ItemType>();
        }
    }
    #endregion
}
