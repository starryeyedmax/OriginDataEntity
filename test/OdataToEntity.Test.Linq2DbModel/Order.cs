//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using OdataToEntity.Linq2Db;
using System.ComponentModel;
using OdataToEntity.Query;

namespace OdataToEntity.Test.Model
{
    static class OpenTypeConverter
    {
        public static readonly String NotSetString = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Database       : OdataToEntity
    /// Data Source    : .\sqlexpress
    /// Server Version : 12.00.5203
    /// </summary>
    public partial class OdataToEntityDB : LinqToDB.Data.DataConnection, IOeLinq2DbDataContext
    {
        public ITable<Category>                Categories              { get { return this.GetTable<Category>(); } }
        public ITable<Customer>                Customers               { get { return this.GetTable<Customer>(); } }
        public ITable<CustomerShippingAddress> CustomerShippingAddress { get { return this.GetTable<CustomerShippingAddress>(); } }
        public ITable<ManyColumns>             ManyColumns             { get { return this.GetTable<ManyColumns>(); } }
        public ITable<ManyColumnsView>         ManyColumnsView         { get { return this.GetTable<ManyColumnsView>(); } }
        public ITable<Order>                   Orders                  { get { return this.GetTable<Order>(); } }
		public ITable<OrderItem>               OrderItems              { get { return this.GetTable<OrderItem>(); } }
		public ITable<OrderItemsView>          OrderItemsView          { get { return this.GetTable<OrderItemsView>(); } }
        public ITable<ShippingAddress>         ShippingAddresses       { get { return this.GetTable<ShippingAddress>(); } }

        OeLinq2DbDataContext IOeLinq2DbDataContext.DataContext
        {
            get;
            set;
        }

		public OdataToEntityDB()
			: base("SqlServer", @"Server=.\sqlexpress;Initial Catalog=OdataToEntity;Trusted_Connection=Yes;")
		{
			InitDataContext();
		}

		partial void InitDataContext();

		#region FreeTextTable

		public class FreeTextKey<T>
		{
			public T   Key;
			public int Rank;
		}
        #endregion

        [Db.OeBoundFunction(CollectionFunctionName = "BoundFunctionCollection", SingleFunctionName = "BoundFunctionSingle")]
        public static IEnumerable<Order> BoundFunction(Db.OeBoundFunctionParameter<Customer, Order> boundParameter, IEnumerable<String> orderNames)
        {
            using (var orderContext = new OdataToEntityDB())
            {
                IQueryable<Customer> customers = boundParameter.ApplyFilter(orderContext.Customers, orderContext);
                IQueryable<Order> orders = customers.SelectMany(c => c.Orders).Where(o => orderNames.Contains(o.Name));

                IQueryable result = boundParameter.ApplySelect(orders, orderContext);
                List<Order> orderList = boundParameter.Materialize(result).ToListAsync().GetAwaiter().GetResult();
                return orderList;
            }
        }
        [Description("dbo.GetOrders")]
        public IEnumerable<Order> GetOrders(int? id, String name, OrderStatus? status) => throw new NotImplementedException();
        [Description("ResetDb()")]
        public void ResetDb() => throw new NotImplementedException();
        [Description("ResetManyColumns()")]
        public void ResetManyColumns() => throw new NotImplementedException();
        [Sql.Function("dbo", "ScalarFunction")]
        public int ScalarFunction() => Orders.Count();
        [Sql.Function("dbo", "ScalarFunctionWithParameters")]
        public int ScalarFunctionWithParameters(int? id, String name, OrderStatus? status) => Orders.Where(o => o.Id == id || o.Name.Contains(name) || o.Status == status).Count();
        [Sql.Function("TableFunction")]
        public IEnumerable<Order> TableFunction() => Orders;
        [Sql.Function("TableFunctionWithCollectionParameter")]
        public IEnumerable<String> TableFunctionWithCollectionParameter(IEnumerable<String> string_list) => string_list;
        [Sql.Function("TableFunctionWithParameters")]
        public IEnumerable<Order> TableFunctionWithParameters(int? id, String name, OrderStatus? status) => Orders.Where(o => (o.Id == id) || o.Name.Contains(name) || (o.Status == status));
    }

    [Table(Schema = "dbo", Name = "Categories")]
    public partial class Category
    {
        [PrimaryKey, Identity]
        public int Id { get; set; } // int
        [Column, NotNull]
        public string Name { get; set; } // varchar(128)
        [Column, Nullable]
        public int? ParentId { get; set; } // int
        [Column, Nullable]
        public DateTime? DateTime { get; set; } //datetime2(7)

        #region Associations

        /// <summary>
        /// FK_Categories_Categories_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "ParentId", CanBeNull = true, IsBackReference = true)]
        [Page(NavigationNextLink = true)]
        public IEnumerable<Category> Children { get; set; }

        /// <summary>
        /// FK_Categories_Categories
        /// </summary>
        [Association(ThisKey = "ParentId", OtherKey = "Id", CanBeNull = true, KeyName = "FK_Categories_Categories", BackReferenceName = "Children")]
        public Category Parent { get; set; }

        #endregion
    }

    [Table(Schema="dbo", Name="Customers")]
	public partial class Customer
	{
        public Customer()
        {
            Address = OpenTypeConverter.NotSetString;
            Country = OpenTypeConverter.NotSetString;
            Id = int.MinValue;
            Name = OpenTypeConverter.NotSetString;
            Sex = (Sex)Int32.MinValue;
        }

		[Column,                Nullable] public string Address { get; set; } // varchar(256)
        [PrimaryKey(Order = 0), NotNull ] public string Country { get; set; } // char(2)
        [PrimaryKey(Order = 1)          ] public int    Id      { get; set; } // int
        [Column,                NotNull ] public string Name    { get; set; } // varchar(128)
		[Column,                Nullable] public Sex?   Sex     { get; set; } // int

		#region Associations
		[Association(ThisKey="Country,Id", OtherKey="AltCustomerCountry,AltCustomerId", CanBeNull=true, IsBackReference=true)]
        [Expand(SelectExpandType.Disabled)]
        public IEnumerable<Order> AltOrders { get; set; }

		[Association(ThisKey="Country,Id", OtherKey="CustomerCountry,CustomerId", CanBeNull=true, IsBackReference=true)]
        [Count(Disabled = true)]
        [Expand(SelectExpandType.Automatic)]
        public IEnumerable<Order> Orders { get; set; }

		[Association(ThisKey="Country,Id", OtherKey="CustomerCountry,CustomerId", CanBeNull=true, IsBackReference=true)]
        public ICollection<CustomerShippingAddress> CustomerShippingAddresses { get; set; }

        public ICollection<ShippingAddress> ShippingAddresses { get; set; }
		#endregion
	}

    public class ManyColumnsBase
    {
        [Column, PrimaryKey] public int Column01 { get; set; } // int
        [Column, NotNull]    public int Column02 { get; set; } // int
        [Column, NotNull]    public int Column03 { get; set; } // int
        [Column, NotNull]    public int Column04 { get; set; } // int
        [Column, NotNull]    public int Column05 { get; set; } // int
        [Column, NotNull]    public int Column06 { get; set; } // int
        [Column, NotNull]    public int Column07 { get; set; } // int
        [Column, NotNull]    public int Column08 { get; set; } // int
        [Column, NotNull]    public int Column09 { get; set; } // int
        [Column, NotNull]    public int Column10 { get; set; } // int
        [Column, NotNull]    public int Column11 { get; set; } // int
        [Column, NotNull]    public int Column12 { get; set; } // int
        [Column, NotNull]    public int Column13 { get; set; } // int
        [Column, NotNull]    public int Column14 { get; set; } // int
        [Column, NotNull]    public int Column15 { get; set; } // int
        [Column, NotNull]    public int Column16 { get; set; } // int
        [Column, NotNull]    public int Column17 { get; set; } // int
        [Column, NotNull]    public int Column18 { get; set; } // int
        [Column, NotNull]    public int Column19 { get; set; } // int
        [Column, NotNull]    public int Column20 { get; set; } // int
        [Column, NotNull]    public int Column21 { get; set; } // int
        [Column, NotNull]    public int Column22 { get; set; } // int
        [Column, NotNull]    public int Column23 { get; set; } // int
        [Column, NotNull]    public int Column24 { get; set; } // int
        [Column, NotNull]    public int Column25 { get; set; } // int
        [Column, NotNull]    public int Column26 { get; set; } // int
        [Column, NotNull]    public int Column27 { get; set; } // int
        [Column, NotNull]    public int Column28 { get; set; } // int
        [Column, NotNull]    public int Column29 { get; set; } // int
        [Column, NotNull]    public int Column30 { get; set; } // int
    }

    [Table(Schema = "dbo", Name = "ManyColumns")]
    public sealed class ManyColumns : ManyColumnsBase
    {
    }

    [Table(Schema = "dbo", Name = "ManyColumnsView")]
    public sealed class ManyColumnsView : ManyColumnsBase
    {
    }

    public abstract class OrderBase
    {
        public OrderBase()
        {
            AltCustomerCountry = OpenTypeConverter.NotSetString;
            AltCustomerId = Int32.MinValue;
            Name = OpenTypeConverter.NotSetString;
        }

        [Column,     Nullable ] public string          AltCustomerCountry { get; set; } // char(2)
        [Column,     Nullable ] public int?            AltCustomerId      { get; set; } // int
        [Select(SelectExpandType.Automatic)]
        [Column,     NotNull  ] public string          Name               { get; set; } // varchar(256)
    }

    [Table(Schema="dbo", Name="Orders")]
    [Count]
    [Page(MaxTop = 2, PageSize = 1)]
	public partial class Order : OrderBase
	{
        public Order()
        {
            CustomerCountry = OpenTypeConverter.NotSetString;
            CustomerId = Int32.MinValue;
            Date = DateTimeOffset.MinValue;
            Id = Int32.MinValue;
            Status = (OrderStatus)Int32.MinValue;
        }

        [Column,     NotNull  ] public string          CustomerCountry    { get; set; } // char(2)
        [Column,     NotNull  ] public int             CustomerId         { get; set; } // int
        [Select(SelectExpandType.Automatic)]
        [Column,     Nullable ] public DateTimeOffset? Date               { get; set; } // datetimeoffset(7)
		[PrimaryKey, Identity ] public int             Id                 { get; set; } // int
        [Select(SelectExpandType.Automatic)]
        [Column,     NotNull  ] public OrderStatus     Status             { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_Orders_AltCustomers
		/// </summary>
		[Association(ThisKey= "AltCustomerCountry,AltCustomerId", OtherKey="Country,Id", CanBeNull=true, KeyName="FK_Orders_AltCustomers", BackReferenceName= "AltOrders")]
		public Customer AltCustomer { get; set; }

		/// <summary>
		/// FK_Orders_Customers
		/// </summary>
		[Association(ThisKey= "CustomerCountry,CustomerId", OtherKey="Country,Id", CanBeNull=false, KeyName="FK_Orders_Customers", BackReferenceName="Orders")]
        [Expand(SelectExpandType.Automatic)]
        [Select("Name", "Sex", SelectType = SelectExpandType.Automatic)]
        public Customer Customer { get; set; }

		/// <summary>
		/// FK_OrderItem_Order_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="OrderId", CanBeNull=true, IsBackReference=true)]
        [Count]
        [Expand(SelectExpandType.Automatic)]
        [OrderBy("Id")]
        [Page(MaxTop = 2, PageSize = 1)]
        public IEnumerable<OrderItem> Items { get; set; }

        /// <summary>
        /// FK_ShippingAddresses_Order_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "OrderId", CanBeNull = true, IsBackReference = true)]
        public IEnumerable<ShippingAddress> ShippingAddresses { get; set; }

        #endregion
    }

    [Table(Schema="dbo", Name="OrderItems")]
    [Count(Disabled = true)]
    [OrderBy(Disabled = true)]
    public partial class OrderItem
	{
        public OrderItem()
        {
            Count = Int32.MinValue;
            Id = Int32.MinValue;
            OrderId = Int32.MinValue;
            Price = Decimal.MinValue;
            Product = OpenTypeConverter.NotSetString;
        }

        [Column,     Nullable ] public int?     Count   { get; set; } // int
        [Filter(Disabled = true)]
        [Select(SelectExpandType.Disabled)]
        [PrimaryKey, Identity ] public int      Id      { get; set; } // int
        [Select(SelectExpandType.Disabled)]
        [Column,     NotNull  ] public int      OrderId { get; set; } // int
		[Column,     Nullable ] public decimal? Price   { get; set; } // decimal(18, 0)
		[Column,     NotNull  ] public string   Product { get; set; } // varchar(256)

		#region Associations

		/// <summary>
		/// FK_OrderItem_Order
		/// </summary>
		[Association(ThisKey="OrderId", OtherKey="Id", CanBeNull=false, KeyName="FK_OrderItem_Order", BackReferenceName="Items")]
		public Order Order { get; set; }

		#endregion
	}

    [Table(Schema = "dbo", Name = "OrderItemsView", IsView = true)]
    public sealed class OrderItemsView
    {
        [Column, NotNull] public String Name    { get; set; }
        [Column, NotNull] public String Product { get; set; }
    }

    [Table(Schema = "dbo", Name = "ShippingAddresses")]
    public sealed class ShippingAddress
    {
        [Column, NotNull              ] public string Address { get; set; } // varchar(256)
        [Column, PrimaryKey(Order = 1)] public int    Id      { get; set; } // int
        [Column, PrimaryKey(Order = 0)] public int    OrderId { get; set; } // int

		#region Associations
		[Association(ThisKey="OrderId,Id", OtherKey="ShippingAddressOrderId,ShippingAddressId", CanBeNull=true, IsBackReference=true)]
        public ICollection<CustomerShippingAddress> CustomerShippingAddresses { get; set; }

        public ICollection<Customer> Customers { get; set; }
		#endregion
    }

    [Table(Schema = "dbo", Name = "CustomerShippingAddress")]
    public sealed class CustomerShippingAddress
    {
        [Column, PrimaryKey(Order = 0), NotNull] public String CustomerCountry        { get; set; }
        [Column, PrimaryKey(Order = 1)         ] public int    CustomerId             { get; set; }
        [Column, PrimaryKey(Order = 2)         ] public int    ShippingAddressOrderId { get; set; }
        [Column, PrimaryKey(Order = 3)         ] public int    ShippingAddressId      { get; set; }

		#region Associations
		[Association(ThisKey="CustomerCountry,CustomerId", OtherKey="Country,Id", CanBeNull=false, KeyName="FK_CustomerShippingAddress_Customers", BackReferenceName="CustomerShippingAddresses")]
        public Customer Customer { get; set; }

        [Association(ThisKey="ShippingAddressOrderId,ShippingAddressId", OtherKey="OrderId,Id", CanBeNull=false, KeyName="FK_CustomerShippingAddress_ShippingAddresses", BackReferenceName="CustomerShippingAddresses")]
        public ShippingAddress ShippingAddress { get; set; }
		#endregion
    }

    public enum OrderStatus
    {
        Unknown,
        Processing,
        Shipped,
        Delivering,
        Cancelled
    }

    public enum Sex
    {
        Male,
        Female
    }

    public class Order2Connection : LinqToDB.Data.DataConnection, IOeLinq2DbDataContext
    {
        public ITable<Customer> Customers2 { get { return this.GetTable<Customer>(); } }
        public ITable<Order>    Orders2    { get { return this.GetTable<Order>(); } }

		public Order2Connection()
			: base("SqlServer", @"Server=.\sqlexpress;Initial Catalog=OdataToEntity;Trusted_Connection=Yes;")
		{
		}

        OeLinq2DbDataContext IOeLinq2DbDataContext.DataContext
        {
            get;
            set;
        }
    }
}
