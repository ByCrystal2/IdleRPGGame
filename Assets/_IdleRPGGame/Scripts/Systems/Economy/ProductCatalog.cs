using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Economy/Product Catalog")]
public class ProductCatalog : ScriptableObject
{
    public List<ProductData> allProducts;
}
