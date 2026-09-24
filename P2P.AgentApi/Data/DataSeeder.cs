using Microsoft.EntityFrameworkCore;
using P2P.AgentApi.Entities;
using P2P.AgentApi.Enums;

namespace P2P.AgentApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(
            AgentApiDbContext db,
            CancellationToken cancellationToken = default
        )
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);

            //  We do not want to re-create the dataset everytime the application starts.
            //  This also prevents duplicate vendors and purchase orders
            if (await db.Vendors.AnyAsync(cancellationToken))
            {
                return;
            }

            //  Using RC Car chassis vendors because it's fun
            var vendors = new[]
            {
                new Vendor(
                    name: "MCD",
                    paymentTerms: PaymentTerms.NET30
                ),

                new Vendor(
                    name: "Tamiya",
                    paymentTerms: PaymentTerms.NET60
                ),

                new Vendor(
                    name: "Usukani",
                    paymentTerms: PaymentTerms.NET30
                ),
                new Vendor(
                    name: "Yokomo",
                    paymentTerms: PaymentTerms.NET60
                )
            };

            await db.Vendors.AddRangeAsync(
                vendors,
                cancellationToken
            );

            await db.SaveChangesAsync(cancellationToken);

            var mcd = vendors[0];
            var tamiya = vendors[1];
            var usukani = vendors[2];
            var yokomo = vendors[3];

            var mcdOrder = new PurchaseOrder(mcd.Id);

            mcdOrder.POLineItems.Add(
                new POLineItem(
                    sku: "OCTOXPRO",
                    description: "MCD OCTOX 1:8 GTE - PRO Chassis Kit",
                    quantityOrdered: 2,
                    unitCost: 1399.99m
                )
            );

            mcdOrder.POLineItems.Add(
                new POLineItem(
                    sku: "04158",
                    description: "BODY SET AUDI RS5 PAINTED",
                    quantityOrdered: 2,
                    unitCost: 419.99m
                )
            );

            var tamiyaOrder = new PurchaseOrder(tamiya.Id);

            tamiyaOrder.POLineItems.Add(
                new POLineItem(
                    sku: "58667",
                    description: "Rc Audi Quattro A2 Tt - 02",
                    quantityOrdered: 1,
                    unitCost: 210.00m
                )
            );

            tamiyaOrder.POLineItems.Add(
                new POLineItem(
                    sku: "53767",
                    description: "Rc 3X10Mm Socket Screw Round Head Blue 5Pcs",
                    quantityOrdered: 5,
                    unitCost: 9.00m
                )
            );

            tamiyaOrder.POLineItems.Add(
                new POLineItem(
                    sku: "87207",
                    description: "MASKING TAPE 2MM",
                    quantityOrdered: 1,
                    unitCost: 0m
                )
            );

            var usukaniOrder = new PurchaseOrder(usukani.Id);

            usukaniOrder.POLineItems.Add(
                new POLineItem(
                    sku: "US88450",
                    description: "USUKANI NGE PRO 1-10 PREMIUM RWD DRIFT CAR CHASSIS KIT [Usukani]",
                    quantityOrdered: 1,
                    unitCost: 549.99m
                )
            );

            var yokomoOrder = new PurchaseOrder(yokomo.Id);

            yokomoOrder.POLineItems.Add(
                new POLineItem(
                    sku: "SDR-030CR",
                    description: "SD 3.0 SUPER DRIFT COMPETITION KIT - LIMITED EDITION RWD 1-10 RC Drift Car kit SD3.0 PINK BRONZE [Yokomo]",
                    quantityOrdered: 3,
                    unitCost: 879.99m
                )
            );

            yokomoOrder.POLineItems.Add(
                new POLineItem(
                    sku: "DR-WING01",
                    description: "DR GT Wing and Mirror Set TYPE 1 - SWAN NECK [Shibata]",
                    quantityOrdered: 1,
                    unitCost: 19.99m
                )
            );

            yokomoOrder.POLineItems.Add(
                new POLineItem(
                    sku: "832301G",
                    description: "Green High Traction 5mm 7mm 1-10 TE37 Style Wheels [MST]",
                    quantityOrdered: 4,
                    unitCost: 8.99m
                )
            );

            db.PurchaseOrders.AddRange(
                mcdOrder,
                tamiyaOrder,
                usukaniOrder,
                yokomoOrder
            );

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}