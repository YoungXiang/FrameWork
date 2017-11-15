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
        public virtual void Setup(DataType data, int index)
        {

        }

        // clean for recycle.
        public virtual void Clean()
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
            // make template as my child, so that template is accessable in OnDestroy;
            //template.transform.SetParent(transform, false);
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

        [Tooltip("Either to pool the elements or not.")]
        public bool usePool = false;
        [Tooltip("This is only relevant when usePool is checked!")]
        public int poolSize = 10;
        protected UnityObjectPool<GameObject> objectPool;
        
        public virtual void SetupDatas(DataType[] datas_)
        {
            ItemType check = template.GetComponent<ItemType>();
            if (check == null)
            {
                Debug.LogError("Template does not have the correct ItemType component!");
                return;
            }

            #region Pool Initialization
            if (usePool)
            {
                objectPool = new UnityObjectPool<GameObject>(poolSize);
                objectPool.createDelegate = () =>
                {
                    return Instantiate(template, transform, false);
                };
                objectPool.deleteDelegate = (GameObject item) =>
                {
                    Destroy(item);
                };
            }
            #endregion

            datas = datas_;
            if (items == null || items.Length != datas.Length)
            {
                if (items != null)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i].Clean();
                        if (usePool)
                        {
                            // recycle item
                            items[i].gameObject.SetActive(false);
                            objectPool.Recycle(items[i].gameObject);
                        }
                        else
                        {
                            Destroy(items[i].gameObject);
                        }
                    }
                }

                items = new ItemType[datas.Length];
                for (int i = 0; i < datas.Length; i++)
                {
                    items[i] = NewItem();
                    items[i].name = string.Format("Item{0}", i);
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
            items[i].Setup(data, i);
        }

        protected virtual void UpdateItems()
        {
            for (int i = 0; i < datas.Length; i++)
            {
                items[i].Setup(datas[i], i);
            }
        }

        protected virtual ItemType NewItem()
        {
            GameObject instanced = usePool ? objectPool.Create() : Instantiate(template, transform, false);
            instanced.SetActive(true);
            return instanced.GetComponent<ItemType>();
        }
        
        protected override void OnDestroy()
        {
            if (usePool)
            {
                objectPool.Destroy();
            }
        }
    }
    #endregion
}
