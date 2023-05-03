using Nop.Core;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara.Services
{
    /// <summary>
    /// Represents the item classification service implementation
    /// </summary>
    public class ItemClassificationService
    {
        #region Fields

        protected readonly AvalaraTaxSettings _avalaraTaxSettings;
        protected readonly IRepository<ItemClassification> _itemClassificationRepository;

        #endregion

        #region Ctor

        public ItemClassificationService(AvalaraTaxSettings avalaraTaxSettings, 
            IRepository<ItemClassification> itemClassificationRepository)
        {
            _avalaraTaxSettings = avalaraTaxSettings;
            _itemClassificationRepository = itemClassificationRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get item classification
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of tax transaction log items
        /// </returns>
        public async Task<IPagedList<ItemClassification>> GetItemClassificationAsync(int? countryId = null,
            int? productId = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //get all items
            var query = _itemClassificationRepository.Table;

            //filter by country
            if (countryId.HasValue && countryId > 0)
                query = query.Where(item => item.CountryId == countryId);

            //filter by product
            if (productId.HasValue)
                query = query.Where(item => item.ProductId == productId);

            //order item records
            query = query.OrderByDescending(item => item.UpdatedOnUtc).ThenByDescending(logItem => logItem.Id);

            //return paged log
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }


        /// <summary>
        /// Get a item classification by the identifier
        /// </summary>
        /// <param name="itemId">Item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log item
        /// </returns>
        public async Task<ItemClassification> GetItemClassificationByIdAsync(int itemId)
        {
            return await _itemClassificationRepository.GetByIdAsync(itemId);
        }

        /// <summary>
        /// Insert the item classification
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains the number of products that were not added
        /// </returns>
        public async Task<int?> InsertItemClassificationAsync(List<int> productIds)
        {
            if (!productIds?.Any() ?? true)
                return productIds.Count;

            var newProductIds = productIds.Except(_itemClassificationRepository.Table.Select(record => record.ProductId)).ToList();
            if (!newProductIds.Any())
                return productIds.Count;

            var countyIds = _avalaraTaxSettings.SelectedCountryIds;
            if (!countyIds?.Any() ?? true)
                return productIds.Count;

            foreach (var countryId in countyIds)
                foreach (var productId in newProductIds)
                {
                    var record = new ItemClassification { 
                        CountryId = countryId, 
                        ProductId = productId, 
                        UpdatedOnUtc = DateTime.UtcNow,
                    };

                    await _itemClassificationRepository.InsertAsync(record, false);
                }
            return productIds.Count - newProductIds.Count;
        }

        /// <summary>
        /// Update the item classification
        /// </summary>
        /// <param name="item">Item classification</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateItemClassificationAsync(ItemClassification item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            await _itemClassificationRepository.UpdateAsync(item, false);
        }

        /// <summary>
        /// Delete records
        /// </summary>
        /// <param name="ids">Items identifiers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteRecordsAsync(List<int> ids)
        {
            await _itemClassificationRepository.DeleteAsync(item => ids.Contains(item.Id));
        }

        /// <summary>
        /// Clear the item classification
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ClearItemClassificationAsync()
        {
            await _itemClassificationRepository.TruncateAsync();
        }

        #endregion
    }
}
