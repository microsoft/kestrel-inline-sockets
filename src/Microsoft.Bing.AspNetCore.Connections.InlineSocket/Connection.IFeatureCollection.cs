// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IFeatureCollection
    {
        internal static readonly Type[] FeatureTypes = new[]
        {
            typeof(IConnectionIdFeature),
            typeof(IConnectionItemsFeature),
            typeof(IConnectionTransportFeature),
            typeof(IMemoryPoolFeature),
            typeof(IConnectionLifetimeFeature),
        };

        bool IFeatureCollection.IsReadOnly => true;

        int IFeatureCollection.Revision => 1;

        private IEnumerable<KeyValuePair<Type, object>> EnumerableFeatures
        {
            get
            {
                foreach (var featureType in FeatureTypes)
                {
                    yield return new KeyValuePair<Type, object>(featureType, this);
                }
            }
        }

        object IFeatureCollection.this[Type key]
        {
            get
            {
                if (FeatureTypes.Contains(key))
                {
                    return this;
                }

                return null;
            }
            set => throw new NotImplementedException();
        }

        TFeature IFeatureCollection.Get<TFeature>()
        {
            if (this is TFeature feature)
            {
                return feature;
            }

            return default;
        }

        void IFeatureCollection.Set<TFeature>(TFeature instance)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<Type, object>> IEnumerable<KeyValuePair<Type, object>>.GetEnumerator() => EnumerableFeatures.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => EnumerableFeatures.GetEnumerator();
    }
}
