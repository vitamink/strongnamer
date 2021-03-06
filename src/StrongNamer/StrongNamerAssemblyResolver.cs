﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace StrongNamer
{
    class StrongNamerAssemblyResolver : BaseAssemblyResolver
    {
        readonly List<AssemblyDefinition> _assemblies = null;

        public StrongNamerAssemblyResolver(IEnumerable<string> assemblyPaths) : base()
        {
            _assemblies = assemblyPaths
                .Select(path => AssemblyDefinition.ReadAssembly(path))
                .ToList();
        }

        // If the base resolver can't resolve an assembly, look for assemblies which are referenced by the project
        // Base resolver checks local folders and the GAC, so will not find anything in the Packages folder
        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            var matchedAssembly = base.Resolve(name, parameters);
            if (matchedAssembly == null)
            {
                matchedAssembly = _assemblies.SingleOrDefault(ad => ad.Name.Name.Equals(name.Name));
            }
            return matchedAssembly;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name, new ReaderParameters());
        }

        public new void Dispose(bool disposing) {
            base.Dispose(disposing);
            for (int i = 0; i < _assemblies.Count; i++) {
                _assemblies.ElementAtOrDefault(i)?.Dispose();
            }
        }
    }
}
