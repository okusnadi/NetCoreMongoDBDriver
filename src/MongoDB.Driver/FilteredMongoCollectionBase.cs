﻿/* Copyright 2015 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoDB.Driver
{
    internal abstract class FilteredMongoCollectionBase<TDocument> : MongoCollectionBase<TDocument>, IFilteredMongoCollection<TDocument>
    {
        // private fields
        private readonly FilterDefinition<TDocument> _filter;
        private readonly IMongoCollection<TDocument> _wrappedCollection;

        // constructors
        public FilteredMongoCollectionBase(IMongoCollection<TDocument> wrappedCollection, FilterDefinition<TDocument> filter)
        {
            _wrappedCollection = wrappedCollection;
            _filter = filter;
        }

        // public properties
        public override CollectionNamespace CollectionNamespace
        {
            get { return _wrappedCollection.CollectionNamespace; }
        }

        public override IMongoDatabase Database
        {
            get { return _wrappedCollection.Database; }
        }

        public override IBsonSerializer<TDocument> DocumentSerializer
        {
            get { return _wrappedCollection.DocumentSerializer; }
        }

        public FilterDefinition<TDocument> Filter
        {
            get { return _filter; }
        }

        public override IMongoIndexManager<TDocument> Indexes
        {
            get { return _wrappedCollection.Indexes; }
        }

        public override MongoCollectionSettings Settings
        {
            get { return _wrappedCollection.Settings; }
        }

        // protected properties
        protected IMongoCollection<TDocument> WrappedCollection
        {
            get { return _wrappedCollection; }
        }

        // public methods
        public override IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filteredPipeline = CreateFilteredPipeline(pipeline);
            return _wrappedCollection.Aggregate(filteredPipeline, options, cancellationToken);
        }

        public override Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filteredPipeline = CreateFilteredPipeline(pipeline);
            return _wrappedCollection.AggregateAsync(filteredPipeline, options, cancellationToken);
        }

        public override BulkWriteResult<TDocument> BulkWrite(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.BulkWrite(CombineModelFilters(requests), options, cancellationToken);
        }

        public override Task<BulkWriteResult<TDocument>> BulkWriteAsync(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.BulkWriteAsync(CombineModelFilters(requests), options, cancellationToken);
        }

        public override long Count(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.Count(CombineFilters(filter), options, cancellationToken);
        }

        public override Task<long> CountAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.CountAsync(CombineFilters(filter), options, cancellationToken);
        }

        public override IAsyncCursor<TField> Distinct<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.Distinct(field, CombineFilters(filter), options, cancellationToken);
        }

        public override Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.DistinctAsync(field, CombineFilters(filter), options, cancellationToken);
        }

        public override IAsyncCursor<TProjection> FindSync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindSync(CombineFilters(filter), options, cancellationToken);
        }

        public override Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindAsync(CombineFilters(filter), options, cancellationToken);
        }

        public override TProjection FindOneAndDelete<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindOneAndDelete(CombineFilters(filter), options, cancellationToken);
        }

        public override Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindOneAndDeleteAsync(CombineFilters(filter), options, cancellationToken);
        }

        public override TProjection FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindOneAndReplace(CombineFilters(filter), replacement, options, cancellationToken);
        }

        public override Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindOneAndReplaceAsync(CombineFilters(filter), replacement, options, cancellationToken);
        }

        public override TProjection FindOneAndUpdate<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindOneAndUpdate(CombineFilters(filter), update, options, cancellationToken);
        }

        public override Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedCollection.FindOneAndUpdateAsync(CombineFilters(filter), update, options, cancellationToken);
        }

        public override IAsyncCursor<TResult> MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            options = options ?? new MapReduceOptions<TDocument, TResult>();
            options.Filter = CombineFilters(options.Filter);
            return _wrappedCollection.MapReduce(map, reduce, options, cancellationToken);
        }

        public override Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            options = options ?? new MapReduceOptions<TDocument, TResult>();
            options.Filter = CombineFilters(options.Filter);
            return _wrappedCollection.MapReduceAsync(map, reduce, options, cancellationToken);
        }

        // private methods
        private FilterDefinition<TDocument> CombineFilters(FilterDefinition<TDocument> filter)
        {
            if (_filter == null)
            {
                return filter;
            }

            if (filter == null)
            {
                return _filter;
            }

            return _filter & filter;
        }

        private IEnumerable<WriteModel<TDocument>> CombineModelFilters(IEnumerable<WriteModel<TDocument>> models)
        {
            return models.Select<WriteModel<TDocument>, WriteModel<TDocument>>(x =>
            {
                switch (x.ModelType)
                {
                    case WriteModelType.DeleteMany:
                        var deleteManyModel = (DeleteManyModel<TDocument>)x;
                        return new DeleteManyModel<TDocument>(CombineFilters(deleteManyModel.Filter));
                    case WriteModelType.DeleteOne:
                        var deleteOneModel = (DeleteOneModel<TDocument>)x;
                        return new DeleteOneModel<TDocument>(CombineFilters(deleteOneModel.Filter));
                    case WriteModelType.InsertOne:
                        return x; // InsertOneModel has no filter
                    case WriteModelType.ReplaceOne:
                        var replaceOneModel = (ReplaceOneModel<TDocument>)x;
                        return new ReplaceOneModel<TDocument>(CombineFilters(replaceOneModel.Filter), replaceOneModel.Replacement) { IsUpsert = replaceOneModel.IsUpsert };
                    case WriteModelType.UpdateMany:
                        var updateManyModel = (UpdateManyModel<TDocument>)x;
                        return new UpdateManyModel<TDocument>(CombineFilters(updateManyModel.Filter), updateManyModel.Update) { IsUpsert = updateManyModel.IsUpsert };
                    case WriteModelType.UpdateOne:
                        var updateOneModel = (UpdateOneModel<TDocument>)x;
                        return new UpdateOneModel<TDocument>(CombineFilters(updateOneModel.Filter), updateOneModel.Update) { IsUpsert = updateOneModel.IsUpsert };
                    default:
                        throw new MongoInternalException("Request type is invalid.");
                }
            });
        }

        private PipelineDefinition<TDocument, TResult> CreateFilteredPipeline<TResult>(PipelineDefinition<TDocument, TResult> pipeline)
        {
            const string matchOperatorName = "$match";

            var filterStage = new DelegatedPipelineStageDefinition<TDocument, TDocument>(
                matchOperatorName,
                (s, sr) =>
                {
                    var renderedFilter = _filter.Render(s, sr);
                    return new RenderedPipelineStageDefinition<TDocument>(matchOperatorName, new BsonDocument(matchOperatorName, renderedFilter), s);
                });

            var filterPipeline = new PipelineStagePipelineDefinition<TDocument, TDocument>(new[] { filterStage });
            var combinedPipeline = new CombinedPipelineDefinition<TDocument, TDocument, TResult>(
                filterPipeline,
                pipeline);

            return new OptimizingPipelineDefinition<TDocument, TResult>(combinedPipeline);
        }
    }
}
