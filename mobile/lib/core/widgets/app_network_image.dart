import 'package:flutter/material.dart';

class AppNetworkImage extends StatelessWidget {
  const AppNetworkImage({
    super.key,
    required this.imageUrl,
    this.fit = BoxFit.contain,
  });
  final String imageUrl;
  final BoxFit fit;

  @override
  Widget build(BuildContext context) => Image.network(
    imageUrl,
    fit: fit,
    loadingBuilder: (context, child, progress) => progress == null
        ? child
        : const Center(child: CircularProgressIndicator()),
    errorBuilder: (context, error, stackTrace) =>
        const Center(child: Icon(Icons.image_not_supported_outlined, size: 40)),
  );
}
