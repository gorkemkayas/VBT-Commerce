// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'product_list_item_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ProductListItemModel {

 String get id;@JsonKey(name: 'name') String get title; String get description;@JsonKey(name: 'categoryId') String get category;@JsonKey(name: 'primaryImageUrl') String get imageUrl; bool get hasVariants;
/// Create a copy of ProductListItemModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ProductListItemModelCopyWith<ProductListItemModel> get copyWith => _$ProductListItemModelCopyWithImpl<ProductListItemModel>(this as ProductListItemModel, _$identity);

  /// Serializes this ProductListItemModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ProductListItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.title, title) || other.title == title)&&(identical(other.description, description) || other.description == description)&&(identical(other.category, category) || other.category == category)&&(identical(other.imageUrl, imageUrl) || other.imageUrl == imageUrl)&&(identical(other.hasVariants, hasVariants) || other.hasVariants == hasVariants));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,title,description,category,imageUrl,hasVariants);

@override
String toString() {
  return 'ProductListItemModel(id: $id, title: $title, description: $description, category: $category, imageUrl: $imageUrl, hasVariants: $hasVariants)';
}


}

/// @nodoc
abstract mixin class $ProductListItemModelCopyWith<$Res>  {
  factory $ProductListItemModelCopyWith(ProductListItemModel value, $Res Function(ProductListItemModel) _then) = _$ProductListItemModelCopyWithImpl;
@useResult
$Res call({
 String id,@JsonKey(name: 'name') String title, String description,@JsonKey(name: 'categoryId') String category,@JsonKey(name: 'primaryImageUrl') String imageUrl, bool hasVariants
});




}
/// @nodoc
class _$ProductListItemModelCopyWithImpl<$Res>
    implements $ProductListItemModelCopyWith<$Res> {
  _$ProductListItemModelCopyWithImpl(this._self, this._then);

  final ProductListItemModel _self;
  final $Res Function(ProductListItemModel) _then;

/// Create a copy of ProductListItemModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? title = null,Object? description = null,Object? category = null,Object? imageUrl = null,Object? hasVariants = null,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,title: null == title ? _self.title : title // ignore: cast_nullable_to_non_nullable
as String,description: null == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String,category: null == category ? _self.category : category // ignore: cast_nullable_to_non_nullable
as String,imageUrl: null == imageUrl ? _self.imageUrl : imageUrl // ignore: cast_nullable_to_non_nullable
as String,hasVariants: null == hasVariants ? _self.hasVariants : hasVariants // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [ProductListItemModel].
extension ProductListItemModelPatterns on ProductListItemModel {
/// A variant of `map` that fallback to returning `orElse`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ProductListItemModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ProductListItemModel() when $default != null:
return $default(_that);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// Callbacks receives the raw object, upcasted.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case final Subclass2 value:
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ProductListItemModel value)  $default,){
final _that = this;
switch (_that) {
case _ProductListItemModel():
return $default(_that);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `map` that fallback to returning `null`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ProductListItemModel value)?  $default,){
final _that = this;
switch (_that) {
case _ProductListItemModel() when $default != null:
return $default(_that);case _:
  return null;

}
}
/// A variant of `when` that fallback to an `orElse` callback.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id, @JsonKey(name: 'name')  String title,  String description, @JsonKey(name: 'categoryId')  String category, @JsonKey(name: 'primaryImageUrl')  String imageUrl,  bool hasVariants)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ProductListItemModel() when $default != null:
return $default(_that.id,_that.title,_that.description,_that.category,_that.imageUrl,_that.hasVariants);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// As opposed to `map`, this offers destructuring.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case Subclass2(:final field2):
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id, @JsonKey(name: 'name')  String title,  String description, @JsonKey(name: 'categoryId')  String category, @JsonKey(name: 'primaryImageUrl')  String imageUrl,  bool hasVariants)  $default,) {final _that = this;
switch (_that) {
case _ProductListItemModel():
return $default(_that.id,_that.title,_that.description,_that.category,_that.imageUrl,_that.hasVariants);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `when` that fallback to returning `null`
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id, @JsonKey(name: 'name')  String title,  String description, @JsonKey(name: 'categoryId')  String category, @JsonKey(name: 'primaryImageUrl')  String imageUrl,  bool hasVariants)?  $default,) {final _that = this;
switch (_that) {
case _ProductListItemModel() when $default != null:
return $default(_that.id,_that.title,_that.description,_that.category,_that.imageUrl,_that.hasVariants);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ProductListItemModel extends ProductListItemModel {
  const _ProductListItemModel({required final  String id, @JsonKey(name: 'name') required final  String title, final  String description = '', @JsonKey(name: 'categoryId') required final  String category, @JsonKey(name: 'primaryImageUrl') final  String imageUrl = '', final  bool hasVariants = false}): super._(id: id, title: title, description: description, category: category, imageUrl: imageUrl, hasVariants: hasVariants);
  factory _ProductListItemModel.fromJson(Map<String, dynamic> json) => _$ProductListItemModelFromJson(json);



/// Create a copy of ProductListItemModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ProductListItemModelCopyWith<_ProductListItemModel> get copyWith => __$ProductListItemModelCopyWithImpl<_ProductListItemModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ProductListItemModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ProductListItemModel&&(identical(other.id, id) || other.id == id)&&(identical(other.title, title) || other.title == title)&&(identical(other.description, description) || other.description == description)&&(identical(other.category, category) || other.category == category)&&(identical(other.imageUrl, imageUrl) || other.imageUrl == imageUrl)&&(identical(other.hasVariants, hasVariants) || other.hasVariants == hasVariants));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,title,description,category,imageUrl,hasVariants);

@override
String toString() {
  return 'ProductListItemModel(id: $id, title: $title, description: $description, category: $category, imageUrl: $imageUrl, hasVariants: $hasVariants)';
}


}

/// @nodoc
abstract mixin class _$ProductListItemModelCopyWith<$Res> implements $ProductListItemModelCopyWith<$Res> {
  factory _$ProductListItemModelCopyWith(_ProductListItemModel value, $Res Function(_ProductListItemModel) _then) = __$ProductListItemModelCopyWithImpl;
@override @useResult
$Res call({
 String id,@JsonKey(name: 'name') String title, String description,@JsonKey(name: 'categoryId') String category,@JsonKey(name: 'primaryImageUrl') String imageUrl, bool hasVariants
});




}
/// @nodoc
class __$ProductListItemModelCopyWithImpl<$Res>
    implements _$ProductListItemModelCopyWith<$Res> {
  __$ProductListItemModelCopyWithImpl(this._self, this._then);

  final _ProductListItemModel _self;
  final $Res Function(_ProductListItemModel) _then;

/// Create a copy of ProductListItemModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? title = null,Object? description = null,Object? category = null,Object? imageUrl = null,Object? hasVariants = null,}) {
  return _then(_ProductListItemModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,title: null == title ? _self.title : title // ignore: cast_nullable_to_non_nullable
as String,description: null == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String,category: null == category ? _self.category : category // ignore: cast_nullable_to_non_nullable
as String,imageUrl: null == imageUrl ? _self.imageUrl : imageUrl // ignore: cast_nullable_to_non_nullable
as String,hasVariants: null == hasVariants ? _self.hasVariants : hasVariants // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
