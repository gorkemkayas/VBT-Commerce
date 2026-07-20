// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'customer_address_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$CustomerAddressModel {

 String get id; String get label; String get recipientName; String get phoneNumber; String get country; String get city; String get district; String get postalCode; String get addressLine1; String? get addressLine2; bool get isDefault;
/// Create a copy of CustomerAddressModel
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$CustomerAddressModelCopyWith<CustomerAddressModel> get copyWith => _$CustomerAddressModelCopyWithImpl<CustomerAddressModel>(this as CustomerAddressModel, _$identity);

  /// Serializes this CustomerAddressModel to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is CustomerAddressModel&&(identical(other.id, id) || other.id == id)&&(identical(other.label, label) || other.label == label)&&(identical(other.recipientName, recipientName) || other.recipientName == recipientName)&&(identical(other.phoneNumber, phoneNumber) || other.phoneNumber == phoneNumber)&&(identical(other.country, country) || other.country == country)&&(identical(other.city, city) || other.city == city)&&(identical(other.district, district) || other.district == district)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.addressLine1, addressLine1) || other.addressLine1 == addressLine1)&&(identical(other.addressLine2, addressLine2) || other.addressLine2 == addressLine2)&&(identical(other.isDefault, isDefault) || other.isDefault == isDefault));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,label,recipientName,phoneNumber,country,city,district,postalCode,addressLine1,addressLine2,isDefault);

@override
String toString() {
  return 'CustomerAddressModel(id: $id, label: $label, recipientName: $recipientName, phoneNumber: $phoneNumber, country: $country, city: $city, district: $district, postalCode: $postalCode, addressLine1: $addressLine1, addressLine2: $addressLine2, isDefault: $isDefault)';
}


}

/// @nodoc
abstract mixin class $CustomerAddressModelCopyWith<$Res>  {
  factory $CustomerAddressModelCopyWith(CustomerAddressModel value, $Res Function(CustomerAddressModel) _then) = _$CustomerAddressModelCopyWithImpl;
@useResult
$Res call({
 String id, String label, String recipientName, String phoneNumber, String country, String city, String district, String postalCode, String addressLine1, String? addressLine2, bool isDefault
});




}
/// @nodoc
class _$CustomerAddressModelCopyWithImpl<$Res>
    implements $CustomerAddressModelCopyWith<$Res> {
  _$CustomerAddressModelCopyWithImpl(this._self, this._then);

  final CustomerAddressModel _self;
  final $Res Function(CustomerAddressModel) _then;

/// Create a copy of CustomerAddressModel
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? label = null,Object? recipientName = null,Object? phoneNumber = null,Object? country = null,Object? city = null,Object? district = null,Object? postalCode = null,Object? addressLine1 = null,Object? addressLine2 = freezed,Object? isDefault = null,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,label: null == label ? _self.label : label // ignore: cast_nullable_to_non_nullable
as String,recipientName: null == recipientName ? _self.recipientName : recipientName // ignore: cast_nullable_to_non_nullable
as String,phoneNumber: null == phoneNumber ? _self.phoneNumber : phoneNumber // ignore: cast_nullable_to_non_nullable
as String,country: null == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String,city: null == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String,district: null == district ? _self.district : district // ignore: cast_nullable_to_non_nullable
as String,postalCode: null == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String,addressLine1: null == addressLine1 ? _self.addressLine1 : addressLine1 // ignore: cast_nullable_to_non_nullable
as String,addressLine2: freezed == addressLine2 ? _self.addressLine2 : addressLine2 // ignore: cast_nullable_to_non_nullable
as String?,isDefault: null == isDefault ? _self.isDefault : isDefault // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [CustomerAddressModel].
extension CustomerAddressModelPatterns on CustomerAddressModel {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _CustomerAddressModel value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _CustomerAddressModel() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _CustomerAddressModel value)  $default,){
final _that = this;
switch (_that) {
case _CustomerAddressModel():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _CustomerAddressModel value)?  $default,){
final _that = this;
switch (_that) {
case _CustomerAddressModel() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String label,  String recipientName,  String phoneNumber,  String country,  String city,  String district,  String postalCode,  String addressLine1,  String? addressLine2,  bool isDefault)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _CustomerAddressModel() when $default != null:
return $default(_that.id,_that.label,_that.recipientName,_that.phoneNumber,_that.country,_that.city,_that.district,_that.postalCode,_that.addressLine1,_that.addressLine2,_that.isDefault);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String label,  String recipientName,  String phoneNumber,  String country,  String city,  String district,  String postalCode,  String addressLine1,  String? addressLine2,  bool isDefault)  $default,) {final _that = this;
switch (_that) {
case _CustomerAddressModel():
return $default(_that.id,_that.label,_that.recipientName,_that.phoneNumber,_that.country,_that.city,_that.district,_that.postalCode,_that.addressLine1,_that.addressLine2,_that.isDefault);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String label,  String recipientName,  String phoneNumber,  String country,  String city,  String district,  String postalCode,  String addressLine1,  String? addressLine2,  bool isDefault)?  $default,) {final _that = this;
switch (_that) {
case _CustomerAddressModel() when $default != null:
return $default(_that.id,_that.label,_that.recipientName,_that.phoneNumber,_that.country,_that.city,_that.district,_that.postalCode,_that.addressLine1,_that.addressLine2,_that.isDefault);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _CustomerAddressModel extends CustomerAddressModel {
  const _CustomerAddressModel({required final  String id, required final  String label, required final  String recipientName, required final  String phoneNumber, required final  String country, required final  String city, required final  String district, required final  String postalCode, required final  String addressLine1, final  String? addressLine2, required final  bool isDefault}): super._(id: id, label: label, recipientName: recipientName, phoneNumber: phoneNumber, country: country, city: city, district: district, postalCode: postalCode, addressLine1: addressLine1, addressLine2: addressLine2, isDefault: isDefault);
  factory _CustomerAddressModel.fromJson(Map<String, dynamic> json) => _$CustomerAddressModelFromJson(json);



/// Create a copy of CustomerAddressModel
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$CustomerAddressModelCopyWith<_CustomerAddressModel> get copyWith => __$CustomerAddressModelCopyWithImpl<_CustomerAddressModel>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$CustomerAddressModelToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _CustomerAddressModel&&(identical(other.id, id) || other.id == id)&&(identical(other.label, label) || other.label == label)&&(identical(other.recipientName, recipientName) || other.recipientName == recipientName)&&(identical(other.phoneNumber, phoneNumber) || other.phoneNumber == phoneNumber)&&(identical(other.country, country) || other.country == country)&&(identical(other.city, city) || other.city == city)&&(identical(other.district, district) || other.district == district)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.addressLine1, addressLine1) || other.addressLine1 == addressLine1)&&(identical(other.addressLine2, addressLine2) || other.addressLine2 == addressLine2)&&(identical(other.isDefault, isDefault) || other.isDefault == isDefault));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,label,recipientName,phoneNumber,country,city,district,postalCode,addressLine1,addressLine2,isDefault);

@override
String toString() {
  return 'CustomerAddressModel(id: $id, label: $label, recipientName: $recipientName, phoneNumber: $phoneNumber, country: $country, city: $city, district: $district, postalCode: $postalCode, addressLine1: $addressLine1, addressLine2: $addressLine2, isDefault: $isDefault)';
}


}

/// @nodoc
abstract mixin class _$CustomerAddressModelCopyWith<$Res> implements $CustomerAddressModelCopyWith<$Res> {
  factory _$CustomerAddressModelCopyWith(_CustomerAddressModel value, $Res Function(_CustomerAddressModel) _then) = __$CustomerAddressModelCopyWithImpl;
@override @useResult
$Res call({
 String id, String label, String recipientName, String phoneNumber, String country, String city, String district, String postalCode, String addressLine1, String? addressLine2, bool isDefault
});




}
/// @nodoc
class __$CustomerAddressModelCopyWithImpl<$Res>
    implements _$CustomerAddressModelCopyWith<$Res> {
  __$CustomerAddressModelCopyWithImpl(this._self, this._then);

  final _CustomerAddressModel _self;
  final $Res Function(_CustomerAddressModel) _then;

/// Create a copy of CustomerAddressModel
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? label = null,Object? recipientName = null,Object? phoneNumber = null,Object? country = null,Object? city = null,Object? district = null,Object? postalCode = null,Object? addressLine1 = null,Object? addressLine2 = freezed,Object? isDefault = null,}) {
  return _then(_CustomerAddressModel(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,label: null == label ? _self.label : label // ignore: cast_nullable_to_non_nullable
as String,recipientName: null == recipientName ? _self.recipientName : recipientName // ignore: cast_nullable_to_non_nullable
as String,phoneNumber: null == phoneNumber ? _self.phoneNumber : phoneNumber // ignore: cast_nullable_to_non_nullable
as String,country: null == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String,city: null == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String,district: null == district ? _self.district : district // ignore: cast_nullable_to_non_nullable
as String,postalCode: null == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String,addressLine1: null == addressLine1 ? _self.addressLine1 : addressLine1 // ignore: cast_nullable_to_non_nullable
as String,addressLine2: freezed == addressLine2 ? _self.addressLine2 : addressLine2 // ignore: cast_nullable_to_non_nullable
as String?,isDefault: null == isDefault ? _self.isDefault : isDefault // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on
