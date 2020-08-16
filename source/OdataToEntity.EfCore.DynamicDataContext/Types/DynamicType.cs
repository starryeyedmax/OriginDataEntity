﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OdataToEntity.EfCore.DynamicDataContext.Types
{
    public abstract class DynamicType
    {
        private readonly DynamicTypeDefinition _dynamicTypeDefinition;

        protected DynamicType(DynamicDbContext dynamicDbContext)
        {
            _dynamicTypeDefinition = dynamicDbContext.TypeDefinitionManager.GetDynamicTypeDefinition(base.GetType());
        }
        protected DynamicType(DynamicTypeDefinition dynamicTypeDefinition)
        {
            _dynamicTypeDefinition = dynamicTypeDefinition;
        }

        [return: MaybeNull]
        private T GetShadowPropertyValue<T>([CallerMemberName] String caller = null!)
        {
            FieldInfo fieldInfo = _dynamicTypeDefinition.GetShadowPropertyFieldInfoNameByGetName(caller);
            return (T)fieldInfo.GetValue(this);

        }
        public void SetValue(String propertyName, Object value)
        {
            FieldInfo fieldInfo = _dynamicTypeDefinition.GetShadowPropertyFieldInfo(propertyName);
            fieldInfo.SetValue(this, value);
        }

#pragma warning disable 0649
        internal IEnumerable<DynamicType>? CollectionNavigation1;
        internal IEnumerable<DynamicType>? CollectionNavigation2;
        internal IEnumerable<DynamicType>? CollectionNavigation3;
        internal IEnumerable<DynamicType>? CollectionNavigation4;
        internal IEnumerable<DynamicType>? CollectionNavigation5;
        internal IEnumerable<DynamicType>? CollectionNavigation6;
        internal IEnumerable<DynamicType>? CollectionNavigation7;
        internal IEnumerable<DynamicType>? CollectionNavigation8;
        internal IEnumerable<DynamicType>? CollectionNavigation9;
        internal IEnumerable<DynamicType>? CollectionNavigation10;
        internal IEnumerable<DynamicType>? CollectionNavigation11;
        internal IEnumerable<DynamicType>? CollectionNavigation12;
        internal IEnumerable<DynamicType>? CollectionNavigation13;
        internal IEnumerable<DynamicType>? CollectionNavigation14;
        internal IEnumerable<DynamicType>? CollectionNavigation15;
        internal IEnumerable<DynamicType>? CollectionNavigation16;
        internal IEnumerable<DynamicType>? CollectionNavigation17;
        internal IEnumerable<DynamicType>? CollectionNavigation18;
        internal IEnumerable<DynamicType>? CollectionNavigation19;
        internal IEnumerable<DynamicType>? CollectionNavigation20;
        internal IEnumerable<DynamicType>? CollectionNavigation21;
        internal IEnumerable<DynamicType>? CollectionNavigation22;
        internal IEnumerable<DynamicType>? CollectionNavigation23;
        internal IEnumerable<DynamicType>? CollectionNavigation24;
        internal IEnumerable<DynamicType>? CollectionNavigation25;
        internal IEnumerable<DynamicType>? CollectionNavigation26;
        internal IEnumerable<DynamicType>? CollectionNavigation27;
        internal IEnumerable<DynamicType>? CollectionNavigation28;
        internal IEnumerable<DynamicType>? CollectionNavigation29;
        internal IEnumerable<DynamicType>? CollectionNavigation30;
        internal IEnumerable<DynamicType>? CollectionNavigation31;
        internal IEnumerable<DynamicType>? CollectionNavigation32;
        internal IEnumerable<DynamicType>? CollectionNavigation33;
        internal IEnumerable<DynamicType>? CollectionNavigation34;
        internal IEnumerable<DynamicType>? CollectionNavigation35;
        internal IEnumerable<DynamicType>? CollectionNavigation36;
        internal IEnumerable<DynamicType>? CollectionNavigation37;
        internal IEnumerable<DynamicType>? CollectionNavigation38;
        internal IEnumerable<DynamicType>? CollectionNavigation39;
        internal IEnumerable<DynamicType>? CollectionNavigation40;
        internal IEnumerable<DynamicType>? CollectionNavigation41;
        internal IEnumerable<DynamicType>? CollectionNavigation42;
        internal IEnumerable<DynamicType>? CollectionNavigation43;
        internal IEnumerable<DynamicType>? CollectionNavigation44;
        internal IEnumerable<DynamicType>? CollectionNavigation45;
        internal IEnumerable<DynamicType>? CollectionNavigation46;
        internal IEnumerable<DynamicType>? CollectionNavigation47;
        internal IEnumerable<DynamicType>? CollectionNavigation48;
        internal IEnumerable<DynamicType>? CollectionNavigation49;
        internal IEnumerable<DynamicType>? CollectionNavigation50;
        internal IEnumerable<DynamicType>? CollectionNavigation51;
        internal IEnumerable<DynamicType>? CollectionNavigation52;
        internal IEnumerable<DynamicType>? CollectionNavigation53;
        internal IEnumerable<DynamicType>? CollectionNavigation54;
        internal IEnumerable<DynamicType>? CollectionNavigation55;
        internal IEnumerable<DynamicType>? CollectionNavigation56;
        internal IEnumerable<DynamicType>? CollectionNavigation57;
        internal IEnumerable<DynamicType>? CollectionNavigation58;
        internal IEnumerable<DynamicType>? CollectionNavigation59;
        internal IEnumerable<DynamicType>? CollectionNavigation60;

        internal DynamicType? SingleNavigation1;
        internal DynamicType? SingleNavigation2;
        internal DynamicType? SingleNavigation3;
        internal DynamicType? SingleNavigation4;
        internal DynamicType? SingleNavigation5;
        internal DynamicType? SingleNavigation6;
        internal DynamicType? SingleNavigation7;
        internal DynamicType? SingleNavigation8;
        internal DynamicType? SingleNavigation9;
        internal DynamicType? SingleNavigation10;
        internal DynamicType? SingleNavigation21;
        internal DynamicType? SingleNavigation22;
        internal DynamicType? SingleNavigation23;
        internal DynamicType? SingleNavigation24;
        internal DynamicType? SingleNavigation25;
        internal DynamicType? SingleNavigation26;
        internal DynamicType? SingleNavigation27;
        internal DynamicType? SingleNavigation28;
        internal DynamicType? SingleNavigation29;
        internal DynamicType? SingleNavigation30;

        internal Object? ShadowProperty1;
        internal Object? ShadowProperty2;
        internal Object? ShadowProperty3;
        internal Object? ShadowProperty4;
        internal Object? ShadowProperty5;
        internal Object? ShadowProperty6;
        internal Object? ShadowProperty7;
        internal Object? ShadowProperty8;
        internal Object? ShadowProperty9;
        internal Object? ShadowProperty10;
        internal Object? ShadowProperty11;
        internal Object? ShadowProperty12;
        internal Object? ShadowProperty13;
        internal Object? ShadowProperty14;
        internal Object? ShadowProperty15;
        internal Object? ShadowProperty16;
        internal Object? ShadowProperty17;
        internal Object? ShadowProperty18;
        internal Object? ShadowProperty19;
        internal Object? ShadowProperty20;
        internal Object? ShadowProperty21;
        internal Object? ShadowProperty22;
        internal Object? ShadowProperty23;
        internal Object? ShadowProperty24;
        internal Object? ShadowProperty25;
        internal Object? ShadowProperty26;
        internal Object? ShadowProperty27;
        internal Object? ShadowProperty28;
        internal Object? ShadowProperty29;
        internal Object? ShadowProperty30;
        internal Object? ShadowProperty31;
        internal Object? ShadowProperty32;
        internal Object? ShadowProperty33;
        internal Object? ShadowProperty34;
        internal Object? ShadowProperty35;
        internal Object? ShadowProperty36;
        internal Object? ShadowProperty37;
        internal Object? ShadowProperty38;
        internal Object? ShadowProperty39;
        internal Object? ShadowProperty40;
        internal Object? ShadowProperty41;
        internal Object? ShadowProperty42;
        internal Object? ShadowProperty43;
        internal Object? ShadowProperty44;
        internal Object? ShadowProperty45;
        internal Object? ShadowProperty46;
        internal Object? ShadowProperty47;
        internal Object? ShadowProperty48;
        internal Object? ShadowProperty49;
        internal Object? ShadowProperty50;
        internal Object? ShadowProperty51;
        internal Object? ShadowProperty52;
        internal Object? ShadowProperty53;
        internal Object? ShadowProperty54;
        internal Object? ShadowProperty55;
        internal Object? ShadowProperty56;
        internal Object? ShadowProperty57;
        internal Object? ShadowProperty58;
        internal Object? ShadowProperty59;
        internal Object? ShadowProperty60;
        internal Object? ShadowProperty61;
        internal Object? ShadowProperty62;
        internal Object? ShadowProperty63;
        internal Object? ShadowProperty64;
        internal Object? ShadowProperty65;
        internal Object? ShadowProperty66;
        internal Object? ShadowProperty67;
        internal Object? ShadowProperty68;
        internal Object? ShadowProperty69;
        internal Object? ShadowProperty70;
        internal Object? ShadowProperty71;
        internal Object? ShadowProperty72;
        internal Object? ShadowProperty73;
        internal Object? ShadowProperty74;
        internal Object? ShadowProperty75;
        internal Object? ShadowProperty76;
        internal Object? ShadowProperty77;
        internal Object? ShadowProperty78;
        internal Object? ShadowProperty79;
        internal Object? ShadowProperty80;
        internal Object? ShadowProperty81;
        internal Object? ShadowProperty82;
        internal Object? ShadowProperty83;
        internal Object? ShadowProperty84;
        internal Object? ShadowProperty85;
        internal Object? ShadowProperty86;
        internal Object? ShadowProperty87;
        internal Object? ShadowProperty88;
        internal Object? ShadowProperty89;
        internal Object? ShadowProperty90;
        internal Object? ShadowProperty91;
        internal Object? ShadowProperty92;
        internal Object? ShadowProperty93;
        internal Object? ShadowProperty94;
        internal Object? ShadowProperty95;
        internal Object? ShadowProperty96;
        internal Object? ShadowProperty97;
        internal Object? ShadowProperty98;
        internal Object? ShadowProperty99;
        internal Object? ShadowProperty100;
        internal Object? ShadowProperty101;
        internal Object? ShadowProperty102;
        internal Object? ShadowProperty103;
        internal Object? ShadowProperty104;
        internal Object? ShadowProperty105;
        internal Object? ShadowProperty106;
        internal Object? ShadowProperty107;
        internal Object? ShadowProperty108;
        internal Object? ShadowProperty109;
        internal Object? ShadowProperty110;
        internal Object? ShadowProperty111;
        internal Object? ShadowProperty112;
        internal Object? ShadowProperty113;
        internal Object? ShadowProperty114;
        internal Object? ShadowProperty115;
        internal Object? ShadowProperty116;
        internal Object? ShadowProperty117;
        internal Object? ShadowProperty118;
        internal Object? ShadowProperty119;
        internal Object? ShadowProperty120;
        internal Object? ShadowProperty121;
        internal Object? ShadowProperty122;
        internal Object? ShadowProperty123;
        internal Object? ShadowProperty124;
        internal Object? ShadowProperty125;
        internal Object? ShadowProperty126;
        internal Object? ShadowProperty127;
        internal Object? ShadowProperty128;
        internal Object? ShadowProperty129;
        internal Object? ShadowProperty130;
        internal Object? ShadowProperty131;
        internal Object? ShadowProperty132;
        internal Object? ShadowProperty133;
        internal Object? ShadowProperty134;
        internal Object? ShadowProperty135;
        internal Object? ShadowProperty136;
        internal Object? ShadowProperty137;
        internal Object? ShadowProperty138;
        internal Object? ShadowProperty139;
#pragma warning restore 0649

#pragma warning disable 8603
        internal T ShadowPropertyGet1<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet2<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet3<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet4<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet5<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet6<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet7<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet8<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet9<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet10<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet11<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet12<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet13<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet14<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet15<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet16<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet17<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet18<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet19<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet20<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet21<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet22<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet23<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet24<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet25<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet26<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet27<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet28<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet29<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet30<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet31<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet32<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet33<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet34<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet35<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet36<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet37<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet38<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet39<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet40<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet41<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet42<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet43<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet44<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet45<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet46<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet47<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet48<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet49<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet50<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet51<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet52<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet53<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet54<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet55<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet56<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet57<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet58<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet59<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet60<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet61<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet62<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet63<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet64<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet65<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet66<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet67<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet68<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet69<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet70<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet71<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet72<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet73<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet74<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet75<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet76<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet77<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet78<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet79<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet80<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet81<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet82<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet83<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet84<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet85<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet86<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet87<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet88<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet89<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet90<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet91<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet92<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet93<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet94<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet95<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet96<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet97<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet98<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet99<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet100<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet101<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet102<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet103<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet104<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet105<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet106<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet107<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet108<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet109<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet110<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet111<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet112<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet113<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet114<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet115<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet116<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet117<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet118<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet119<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet120<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet121<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet122<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet123<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet124<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet125<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet126<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet127<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet128<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet129<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet130<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet131<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet132<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet133<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet134<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet135<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet136<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet137<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet138<T>() => GetShadowPropertyValue<T>();
        internal T ShadowPropertyGet139<T>() => GetShadowPropertyValue<T>();
#pragma warning restore 8603
    }
}
